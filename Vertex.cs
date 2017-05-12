using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex  {

	private float x;
	private float y;
	private float z;

	public Vertex(float _x, float _y,float _z){
		this.x = _x;
		this.y = _y;
		this.z = _z;
	}

	public Vertex(Vertex _vertex){
	
		this.x = _vertex.getX ();
		this.y = _vertex.getY ();
		this.z = _vertex.getZ ();
	}


	public float getX(){
		return x;
	}

	public float getY(){
		return y;
	}

	public float getZ(){
		return z;
	}

	public void setX(float _x){
		this.x = _x;
	}

	public void setY(float _y){
		this.y = _y;
	}

	public void setZ(float _z){
		this.z = _z;
	}




}
