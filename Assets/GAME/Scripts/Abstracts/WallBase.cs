using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DinoFracture;
public abstract class WallBase : MonoBehaviour
{
    public float maxHp;
    public float currentHp;

    public Transform stackOffSet;

    [SerializeField] long breakIncome;

    [SerializeField] List<WallPiece> wallPieces = new List<WallPiece>();

    [SerializeField] Collider collider;

    [SerializeField] Transform pieceParent;

    bool isDied = false;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0 && !isDied)
        {
            collider.enabled = false;

            isDied = true;

            currentHp = 0;

            EventManager.OnGettingWall?.Invoke();

            EventManager.OnSettingHeight?.Invoke();

            GiveBreakingIncome();

            ExplodePieces();

            StartCoroutine(Kill());

            return; 
        }

        ProceduralBreaking(damage);
    }
     
    public IEnumerator Kill()
    {
        yield return new WaitForSeconds(3f);

        gameObject.SetActive(false);

        ResetWall();

        ResetWallPieces();
    }

    public void ResetWall()
    {
        currentHp = maxHp; 
        
        collider.enabled = true;

        isDied = false;
    }

    private void ResetWallPieces()
    {
        foreach (WallPiece wallPiece in wallPieces)
        {
            wallPiece.ResetPiece();
        }
    }

    void ProceduralBreaking(float damage)
    {
        foreach (WallPiece wallPiece in wallPieces)
        {
            wallPiece.ProceduralBreaking(damage / maxHp);
        }
    }

    void ExplodePieces()
    {
        foreach (WallPiece wallPiece in wallPieces)
        {
              wallPiece.Explode(pieceParent.position);
        }
    }

    public void GiveBreakingIncome()
    {
        EventManager.OnGettingBreakingIncome?.Invoke(breakIncome);
    }

    //private IEnumerator IEResetWall()
    //{
    //    yield return new WaitForSeconds(3f);

    //    isDied = false;

    //    ResetWall();

    //    ResetWallPieces();

    //    EventManager.OnReplacingWall(this);
    //}
}