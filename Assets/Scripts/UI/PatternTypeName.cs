using UnityEngine;

public class PatternTypeName : MonoBehaviour
{
    [SerializeField] private string fullPatternTypeName;
    public string FullPatternTypeName { get { return fullPatternTypeName; } }
}
