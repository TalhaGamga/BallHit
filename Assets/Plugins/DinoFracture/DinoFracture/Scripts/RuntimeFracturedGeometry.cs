using UnityEngine;
using System.Collections;

namespace DinoFracture
{
    /// <summary>
    /// Apply this component to any game object you wish to fracture while
    /// running in game mode.  Runtime fractures will produce a unique 
    /// set of pieces with each fracture.  However, this is at the cost of 
    /// computational time.  It is recommended that both the piece count
    /// and poly count are kept low.  This component is most effective when
    /// FractureRadius is set to a value in-between 0 and 1.
    /// </summary>
    public class RuntimeFracturedGeometry : FractureGeometry
    {
        /// <summary>
        /// If true, the fracture operation is performed on a background thread 
        /// and may not be finished by the time the fracture call returns. A
        /// couple of frames can go by from the time of the fracture to when
        /// the pieces are ready.  If this is false, the fracture will guaranteed
        /// be complete by the end of the call, but the game will be paused while
        /// the fractures are being created.  
        /// </summary>
        /// <remarks>It is recommended to set asynchronous to true whenever possible.</remarks>
        public bool Asynchronous = true;

        protected override AsyncFractureResult FractureInternal(Vector3 localPos)
        {
            FractureDetails details = new FractureDetails();
            details.NumPieces = NumFracturePieces;
            details.NumIterations = NumIterations;
            details.UVScale = FractureUVScale.Piece;
            details.FractureCenter = localPos;
            details.FractureRadius = FractureRadius;
            details.Asynchronous = Asynchronous;

            return Fracture(details, true);
        }
    }
}