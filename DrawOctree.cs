using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOctree : MonoBehaviour {

	[Tooltip("En caso de que un modelo este formado por varias mesh")]
	public Mesh [] mesh;


	[Tooltip("Nivel maximo de profundidad del octree")]
	public int maxLevel;

	[Tooltip("Numero maximo de objetos por AABB")]
	public int maxObjectsLevel;

	//Octree
	private Octree octree;

	//Lista de las AABB para su futura representacion
	private List<AABB> listAABB;

	//Lista de vertices
	private List<Vertex> vertexList;

	//Max y min de la AABB
	private Vertex minAABB;
	private Vertex maxAABB;


	// Use this for initialization
	void Start () {	

	    vertexList = new List<Vertex> ();

		maxAABB = new Vertex (Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);

		minAABB = new Vertex (Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

		Vector3 vectorTransform = new Vector3 ();

		//Por cada mesh se guardan todos los vertices
		foreach(Mesh meshAux in mesh){			

			foreach(Vector3 vectorAux in meshAux.vertices){

				//---- Localizar el vertice acorde la posicion del modelo en el worldspace
				vectorTransform = transform.TransformPoint (vectorAux);

				//--- Guardar los vertices
				vertexList.Add (new Vertex(vectorTransform.x,vectorTransform.y,vectorTransform.z));
					
				 
				//-------Establecer la x min/max------
				if(minAABB.getX()>vectorTransform.x){
					minAABB.setX (vectorTransform.x);
				}

				if (maxAABB.getX () < vectorTransform.x) {
					maxAABB.setX (vectorTransform.x);
				}

				//-------Establecer la y min/max------
				if(minAABB.getY()>vectorTransform.y){
					minAABB.setY (vectorTransform.y);
				}

				if (maxAABB.getY () < vectorTransform.y) {
					maxAABB.setY (vectorTransform.y);
				}

				//-------Establecer la z min/max------
				if(minAABB.getZ()>vectorTransform.z){
					minAABB.setZ (vectorTransform.z);
				}

				if (maxAABB.getZ () < vectorTransform.z) {
					maxAABB.setZ (vectorTransform.z);
				}


			}

		}

		//Se genera el Octree
		octree = new Octree (vertexList,maxLevel,maxObjectsLevel,minAABB,maxAABB);

		//Se cargan las AABB generadas por el octree
		listAABB = octree.getAABB ();


	}
	



	void OnDrawGizmos(){
		
		//Dibuja las AABB
		if (listAABB != null) {
		
			foreach(AABB aabbAux in listAABB){
				aabbAux.DrawAABB (Color.black);
			}

		}

		//Dibuja los vertices del modelo
		if (vertexList != null) {

			foreach (Vertex aux in vertexList) {
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (new Vector3 (aux.getX (), aux.getY (), aux.getZ ()), 0.02f);

			}

		}

		//Dibuja los vertices MAX Y MIN
		if(minAABB!=null && maxAABB!=null){

			Gizmos.color =Color.green;
			Gizmos.DrawSphere (new Vector3 (minAABB.getX (), minAABB.getY (), minAABB.getZ ()), 0.05f);

			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere (new Vector3 (maxAABB.getX (), maxAABB.getY (), maxAABB.getZ ()), 0.05f);
		}

	}


	public int getProfundidad(){
		return maxLevel;
	}

	public int getMaxObjNodo(){
		return maxObjectsLevel;
	}

	public bool isOptimus(){
		return octree.isOptimus ();
	}

}
