using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ML_Agents.Examples.SoccerNew.Scripts
{
    public class AgentSound : AgentSoccerHeadTurn
    {
        private SoccerEnvController m_EnvController;

        public override void Initialize()
        {
            m_EnvController = GetComponentInParent<SoccerEnvController>();
            base.Initialize();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // 12 floats
            sensor.AddObservation(GetSoundObservations());

            // 12 floats total
        }

        /// <summary>
        /// Get the velocity of the agent.
        /// The velocity is the magnitude of the agent's Rigidbody velocity.
        /// </summary>
        /// <returns>The velocity of the agent</returns>
        public static float AgentVelocity(AgentSoccer a)
        {
            return a.agentRb.velocity.magnitude;
        }

        /// <summary>
        /// Get the sound observations for the agent. The observations are the relative position of nearby
        /// entities that are moving at a velocity greater than the minimumAudibleVelocity.
        /// </summary>
        ///
        /// <remarks>
        /// maxAudibleEntities * 3 floats (x, y, z)
        /// </remarks>
        ///
        /// <param name="maxAudibleEntities">The maximum number of entities that can be heard at once</param>
        /// <param name="hearingDistance">The maximum distance at which entities can be heard</param>
        /// <param name="minimumAudibleVelocity">The minimum velocity of an entity for it to be heard</param>
        /// <returns>An array of floats representing the sound observations</returns>
        float[] GetSoundObservations(int maxAudibleEntities = 4, float hearingDistance = 10.0f,
            float minimumAudibleVelocity = 1.0f)
        {
            float[] observations = new float[maxAudibleEntities * 3];
            int lastFreeIndex = 0;


            GameObject ball = m_EnvController.ball;
            var agentsList = m_EnvController.AgentsList;

            float distanceToBall = Vector3.Distance(gameObject.transform.position, ball.transform.position);
            if (distanceToBall <= hearingDistance)
            {
                Rigidbody ballRb = m_EnvController.ballRb;
                if (ballRb.velocity.magnitude >= minimumAudibleVelocity)
                {
                    if (lastFreeIndex + 2 < observations.Length)
                    {
                        Vector3 relativePosition = ball.transform.position - gameObject.transform.position;

                        observations[lastFreeIndex] = relativePosition.x;
                        observations[lastFreeIndex + 1] = relativePosition.y;
                        observations[lastFreeIndex + 2] = relativePosition.z;
                    }
                }
            }

            lastFreeIndex += 3;

            foreach (var playerInfo in agentsList)
            {
                if (lastFreeIndex + 2 >= observations.Length) break;

                AgentSoccer agent = playerInfo.Agent;
                if (!agent || agent == this) continue;

                float distanceToAgent = Vector3.Distance(gameObject.transform.position, agent.transform.position);
                if (distanceToAgent > hearingDistance) continue;

                if (AgentVelocity(agent) >= minimumAudibleVelocity)
                {
                    Vector3 relativePosition = agent.transform.position - gameObject.transform.position;

                    //Debug.DrawRay(gameObject.transform.position, relativePosition, Color.green, 0.5f, false);

                    observations[lastFreeIndex] = relativePosition.x;
                    observations[lastFreeIndex + 1] = relativePosition.y;
                    observations[lastFreeIndex + 2] = relativePosition.z;
                    lastFreeIndex += 3;
                }
            }

            // print("ball position: " + ball.transform.position);
            // print("agent count: " + AgentsList.Count);
            // print("distance to ball: " + Vector3.Distance(gameObject.transform.position, ball.transform.position));
            // print("arr: " + string.Join(", ", observations));

            return observations;
        }
    }
}
