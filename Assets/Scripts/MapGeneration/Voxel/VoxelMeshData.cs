using UnityEngine;
using System.Linq;

namespace MapGeneration.Voxel
{
	public static class VoxelMeshData
	{
		public static readonly Vector3[] Vertices = new Vector3[]
		{
			//whole numbers vertices
			new Vector3(0, 0, 0),
			new Vector3(1, 0, 0),
			new Vector3(1, 1, 0),
			new Vector3(0, 1, 0),
			new Vector3(0, 0, 1),
			new Vector3(1, 0, 1),
			new Vector3(1, 1, 1),
			new Vector3(0, 1, 1),
		};

		public static readonly int[][] Faces = new int[][]
		{
			//north
			new[] {4, 5, 6, 7}, // backface
			//east
			new[] {1, 2, 6, 5},
			//south
			new[] {0, 3, 2, 1}, 
			//west
			new[] {0, 4, 7, 3}, // backface
			//up
			new[] {2, 3, 7, 6},
			//down
			new[] {0, 1, 5, 4}, // backface
		};

		public static Vector3[] FaceVertices(int faceIndex, Vector3 scale, Vector3 position)
		{
			var vertices = new Vector3[4];
			var face = Faces[faceIndex];
			for (var i = 0; i < 4; i++)
			{
				Vector3 vertex = Vertices[face[i]];
				vertex.x *= scale.x;
				vertex.y *= scale.y;
				vertex.z *= scale.z;
				vertices[i] = vertex + position;
			}

			return vertices;
		}
	}
}