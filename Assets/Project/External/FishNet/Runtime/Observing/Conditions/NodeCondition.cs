using FishNet.Connection;
using FishNet.Observing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishNet.Component.Observing
{
    /// <summary>
    /// This condition makes an object only visible to the server.
    /// </summary>
    [CreateAssetMenu(menuName = "FishNet/Observers/Same Node Condition", fileName = "New Node Condition")]
    public class NodeCondition : ObserverCondition
    {
        Dictionary<NetworkConnection, int> ConnectionToNode = new();//-1 Indicates null

        /// <summary>
        /// Returns if the object which this condition resides should be visible to connection.
        /// </summary>
        /// <param name="connection">Connection which the condition is being checked for.</param>
        /// <param name="currentlyAdded">True if the connection currently has visibility of this object.</param>
        /// <param name="notProcessed">True if the condition was not processed. This can be used to skip processing for performance. While output as true this condition result assumes the previous ConditionMet value.</param>
        public override bool ConditionMet(NetworkConnection connection, bool currentlyAdded, out bool notProcessed)
        {
            notProcessed = false;

            if (!ConnectionToNode.ContainsKey(connection))
            {
                ConnectionToNode.Add(connection, -1);
            }
            if (!ConnectionToNode.ContainsKey(NetworkObject.Owner))
            {
                ConnectionToNode.Add(connection, -1);
            }
            if (ConnectionToNode[connection] == -1 || ConnectionToNode[NetworkObject.Owner] == -1) { return false; }

            return ConnectionToNode[connection] == ConnectionToNode[NetworkObject.Owner];
        }

        /// <summary>
        /// True if the condition requires regular updates.
        /// </summary>
        /// <returns></returns>
        public override bool Timed()
        {
            return true;
        }

        /// <summary>
        /// Clones referenced ObserverCondition. This must be populated with your conditions settings.
        /// </summary>
        /// <returns></returns>
        public override ObserverCondition Clone()
        {
            NodeCondition copy = ScriptableObject.CreateInstance<NodeCondition>();
            return copy;
        }
    }
}

