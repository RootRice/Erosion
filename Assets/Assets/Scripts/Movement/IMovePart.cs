using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovePart
{
    public void Init();
    public IEnumerator Run();
    public void Stop();
}
