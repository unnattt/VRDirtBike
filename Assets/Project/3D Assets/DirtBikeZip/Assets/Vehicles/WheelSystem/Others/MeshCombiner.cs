using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshCombiner : MonoBehaviour
{
    void Awake()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>(0);

        transform.gameObject.SetActive(false);
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 localScale = transform.localScale;
        Transform parent = transform.parent;

        transform.parent = null;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        int i = 0;
        while (i < meshFilters.Length)
        {
            if (meshFilters[i].sharedMesh)
            {
                CombineInstance instance = new CombineInstance();
                instance.mesh = meshFilters[i].sharedMesh;
                instance.transform = meshFilters[i].transform.localToWorldMatrix;
                if (meshFilters[i].gameObject != gameObject)
                {
                    Destroy(meshFilters[i].gameObject);
                }
                combine.Add(instance);
            }
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
        transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;

        transform.parent = parent;
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = localScale;

        transform.gameObject.SetActive(true);
    }
}