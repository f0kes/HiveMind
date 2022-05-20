using System;
using System.Collections.Generic;
using MapGeneration.BSP;
using UnityEngine;

namespace MapGeneration.Voxel
{
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class VoxelRenderer : MonoBehaviour
	{
		private MeshCollider _meshCollider;
		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;
		private Mesh _mesh;

		private List<Vector3> _vertices = new List<Vector3>();
		private List<int> _triangles = new List<int>();
		private static readonly int ColorProperty = Shader.PropertyToID("_Color");


		private void Awake()
		{
			_meshRenderer = GetComponent<MeshRenderer>();
			_meshFilter = GetComponent<MeshFilter>();
			_meshCollider = GetComponent<MeshCollider>();
			_mesh = _meshFilter.mesh;
		}
		
		public void SetColor(Color color)
		{
			Material newMaterial = new Material(_meshRenderer.material);
			newMaterial.SetColor(ColorProperty,color);
			_meshRenderer.material = newMaterial;
		}

		public void RenderVoxels(TileMap toDraw, Vector3 voxelScale, Vector3 position)
		{
			_vertices.Clear();
			_triangles.Clear();

			for (var x = 0; x < toDraw.XSize; x++)
			{
				for (var z = 0; z < toDraw.ZSize; z++)
				{
					if (toDraw[x, z] == (int)TileType.Wall)
					{
						Vector3 voxelPosition = new Vector3(x * voxelScale.x, 0, z * voxelScale.z) + position;
						MakeVoxel(toDraw, x, z, voxelScale, voxelPosition);
					}
				}
			}

			UpdateMesh();
		}

		private void MakeVoxel(TileMap tileMap, int x, int z, Vector3 scale, Vector3 position)
		{
			for (int i = 0; i < 6; i++)
			{
				var direction = (Direction) i;
				if (tileMap.GetRootNeighbour(x, z, direction) != 1)
					MakeFace(direction, scale, position);
			}
		}

		private void MakeFace(Direction dir, Vector3 scale, Vector3 position)
		{
			_vertices.AddRange(VoxelMeshData.FaceVertices((int) dir, scale, position));

			int baseIndex = _vertices.Count - 4;
			_triangles.Add(baseIndex);
			_triangles.Add(baseIndex + 1);
			_triangles.Add(baseIndex + 2);
			_triangles.Add(baseIndex);
			_triangles.Add(baseIndex + 2);
			_triangles.Add(baseIndex + 3);
		}

		private void UpdateMesh()
		{
			_mesh.Clear();
			_mesh.vertices = _vertices.ToArray();
			_mesh.triangles = _triangles.ToArray();
			_mesh.RecalculateNormals();
			_meshCollider.sharedMesh = _mesh;
		}
	}
}