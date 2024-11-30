using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ML_Agents.Examples.SoccerNew.Scripts
{
    public class AgentSoccerHeadTurn : AgentSoccer
    {
        [Tooltip("The raycast sensor for the agent that points forward.")]
        public RayPerceptionSensorComponent3D forwardRay;

        [Tooltip("The speed at which the agent's head turns.")]
        public float headTurnSpeed = 100f;

        [Tooltip("The maximum angle the agent's head can turn.")]
        public float maxHeadTurnAngle = 60f;

        /// <summary>
        /// Move the head of the agent based on the input of the neural network.
        /// </summary>
        /// <param name="headRotateAxis">The axis to rotate the head on</param>
        private void MoveHeadInput(int headRotateAxis)
        {
            var headRotateDir = headRotateAxis switch
            {
                1 => forwardRay.transform.up * -1f,
                2 => forwardRay.transform.up * 1f,
                _ => Vector3.zero
            };

            forwardRay.transform.Rotate(headRotateDir, Time.deltaTime * headTurnSpeed);

            if (Vector3.Angle(transform.forward, forwardRay.transform.forward) > maxHeadTurnAngle)
            {
                forwardRay.transform.Rotate(-headRotateDir, Time.deltaTime * headTurnSpeed);
            }
        }

        public override void MoveAgent(ActionSegment<int> act)
        {
            MoveHeadInput(act[3]);
            base.MoveAgent(act);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;

            // Move head
            if (Input.GetKey(KeyCode.Z))
            {
                discreteActionsOut[3] = 1;
            }

            if (Input.GetKey(KeyCode.X))
            {
                discreteActionsOut[3] = 2;
            }

            base.Heuristic(actionsOut);
        }

        public override void OnEpisodeBegin()
        {
            forwardRay.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            base.OnEpisodeBegin();
        }
    }
}
