using System.Collections.Generic;
using UnityEngine;

#region Simple UI Pool
public class UIPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform hiddenRoot;
    private readonly Stack<T> stack = new();

    public UIPool(T prefab, int prewarm, Transform hiddenRoot)
    {
        this.prefab = prefab;
        this.hiddenRoot = hiddenRoot;

        for (int i = 0; i < prewarm; i++)
        {
            var inst = Object.Instantiate(prefab, hiddenRoot);
            inst.gameObject.SetActive(false);
            stack.Push(inst);
        }
    }

    public T Get(Transform parent)
    {
        var inst = stack.Count > 0 ? stack.Pop() : Object.Instantiate(prefab);//true = Delete
        inst.transform.SetParent(parent, false);
        inst.gameObject.SetActive(true);
        return inst;
    }

    public void Release(T inst)
    {
        inst.gameObject.SetActive(false);
        inst.transform.SetParent(hiddenRoot, false);
        stack.Push(inst);
    }

    public void ReleaseAll(List<T> actives)
    {
        foreach (var a in actives) Release(a);
        actives.Clear();
    }
}
#endregion

