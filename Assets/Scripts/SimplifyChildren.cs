using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class SimplifyChildren : MonoBehaviour
{
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>()
            .Where(mf => mf.gameObject != this.gameObject)
            .ToArray();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        MeshRenderer[] childMeshRenderers = GetComponentsInChildren<MeshRenderer>()
            .Where(mr => mr.gameObject != this.gameObject)
            .ToArray();
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.GetComponent<MeshCollider>().sharedMesh = mesh;
        transform.GetComponent<MeshRenderer>().materials = childMeshRenderers[0].materials;
        transform.gameObject.SetActive(true);
    }
}