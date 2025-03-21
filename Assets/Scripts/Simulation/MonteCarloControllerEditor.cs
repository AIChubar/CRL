using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonteCarloController))]
public class MonteCarloControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); 

        MonteCarloController gameManager = (MonteCarloController)target;

        if (GUILayout.Button("Run Monte Carlo Simulation"))
        {
            gameManager.RunMonteCarloSimulation();
        }
    }
}