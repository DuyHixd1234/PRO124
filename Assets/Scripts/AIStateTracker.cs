using System.Collections.Generic;
using UnityEngine;

public static class AIStateTracker
{
    // Lưu trạng thái mỗi AI giữa các pha
    public static Dictionary<string, bool> PreviousStates = new();
}
