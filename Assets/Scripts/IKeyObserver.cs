using UnityEngine;
using System.Collections.Generic;

public interface IKeyObserver
{
    void OnKeyCollected(GameObject collision);
}
