using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class SimplifyChildren : MonoBehaviour
{
    private float quality = 1f;
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
            combine[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.GetComponent<MeshCollider>().sharedMesh = mesh;
        transform.GetComponent<MeshRenderer>().materials = childMeshRenderers[0].materials;
        transform.gameObject.SetActive(true);
        SimplifyMeshFilter(transform.GetComponent<MeshFilter>());
    }
    
    private void SimplifyMeshFilter(MeshFilter meshFilter)
    {
        Mesh sourceMesh = meshFilter.sharedMesh;
        if (sourceMesh == null) // verify that the mesh filter actually has a mesh
            return;

        // Create our mesh simplifier and setup our entire mesh in it
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);

        // This is where the magic happens, lets simplify!
        meshSimplifier.SimplifyMesh(quality);

        // Create our final mesh and apply it back to our mesh filter
        meshFilter.sharedMesh = meshSimplifier.ToMesh();
    }
}