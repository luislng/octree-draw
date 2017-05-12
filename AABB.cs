using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB {

	 
	private Vector3 pos;
	private Vector3 size;
	private Vertex maxAABB;
	private Vertex minAABB; 

	public AABB(Vertex min,Vertex max){
	
		this.maxAABB = new Vertex(max);
		this.minAABB = new Vertex(min);

		size = new Vector3 ((max.getX()-min.getX()),(max.getY()-min.getY()),(max.getZ()-min.getZ()));
		pos = new Vector3 (((max.getX()+min.getX())/2.0f),((max.getY()+min.getY())/2.0f),((max.getZ()+min.getZ())/2.0f));
				   
	}

	public void DrawAABB(Color color){
		
		Gizmos.color = color;
		Gizmos.DrawWireCube (pos,size);
	
	}

	public Vertex getMaxAABB(){
		return maxAABB;
	}

	public Vertex getMinAABB(){
		return minAABB;
	}


}
