using UnityEngine;

[CreateAssetMenu(menuName = "Objective")]
public class Objective : ScriptableObject
{
    public Goal goal;

    [TextArea(2, 5)]
    public string objective;
    public bool active = false;
    public bool complete = false;

    public Objective(string objective)
    {
        this.objective = objective;
        complete = false;
        active = false;
    }
}
