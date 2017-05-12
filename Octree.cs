using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree {

	private class Node{

		//Referencia al padre
		private Node father;

		//Lista de elementos
		private List<Vertex> vertexList;

		//AABB
		private Vertex minAABB;
		private Vertex maxAABB;

		//Nivel del nodo
		private int level;

		//Hijos
		private Node[] children;
		private bool isChildrenCreated;

		//Indica que el nodo se encuentra a la profundidad maxima
		private bool isFinalNode;

		//Numero maximo de objetos por nodo
		private int maxObjects; 

		//Nivel maximo definido por el usuario
		private int maxLevel;

		//Nivel maximo definido por la estructura en caso de tener que seguir dividiendo
		private int limitLevel;


		public Node(){}

		public Node(Node _father,int _level,Vertex _maxAABB,Vertex _minAABB,int _maxObjects,int _maxLevel,int _limitLevel){

			this.father=_father;

			this.level=_level;

			this.maxAABB=_maxAABB;

			this.minAABB=_minAABB;

			this.maxLevel=_maxLevel;

			//Indica que se ha llegado a nodo hoja y hay que almacenar el dato
			isFinalNode=(level>=_maxLevel);

			isChildrenCreated=false;

			this.maxObjects=_maxObjects;

			this.limitLevel=_limitLevel;

			if(isFinalNode){
				vertexList=new List<Vertex>();
			}

		}


		public AABB getAABB(){
			return new AABB (minAABB,maxAABB);
		}


		public void insertarDato(Vertex p){

			//Si es nodo final, significa que he llegado al nivel establecido y almaceno el objeto
			if(isFinalNode){    

				//Si el contenedor supera el numero maximo de objetos, hay que volver a dividir
				if(vertexList.Count>maxObjects){

					//Si se iguala el limitLevel, dejo de dividir e indico que el octree no es optimo 
					if(level==limitLevel){

						creacionStatus=false;
						vertexList.Add(p);

					}else{

						isFinalNode=false;

						//Genero los hijos
						generarHijos();

						//Redistribuyo los datos entre los hijos
						foreach(Vertex vertexAux in vertexList){

							seleccionarHijo(vertexAux);
						}

						//Introduzco el ultimo dato
						seleccionarHijo(p);

						//Elimino el contenedor del nodo actual --------------->>¿LA ELIMINACION ES CORRECTA?
						vertexList=null;
					}                    

				}else{
					vertexList.Add(p);
				}


			}else{

				//Caso contrario genero 8 hijos en caso de no estar ya generados
				if(!isChildrenCreated){
					generarHijos();
				}

				//Seleccionar el hijo al que se le va a introducir el dato
				seleccionarHijo(p);

			}

		}

		private void seleccionarHijo(Vertex p){

			float coorX=p.getX(); float coorY=p.getY();
			float coorZ=p.getZ();

			float minX=minAABB.getX();  float minY=minAABB.getY();
			float minZ=minAABB.getZ();
			float maxX=maxAABB.getX();  float maxY=maxAABB.getY();
			float maxZ=maxAABB.getZ();
			float medX=(maxX+minX)/2.0f; float medY=(maxY+minY)/2.0f;
			float medZ=(maxZ+minZ)/2.0f;

			if(coorY >= medY){

				if(coorX >= medX){
					if(coorZ >= medZ){
						children[7].insertarDato(p); 
					}
					else{ 
						children[5].insertarDato(p); 
					}
				}else {
					if(coorZ >= medZ){ 
						children[6].insertarDato(p); 
					}
					else { 
						children[4].insertarDato(p);
					}
				}
			}else{
				if(coorX >= medX){

					if(coorZ >= medZ){ 
						children[3].insertarDato(p); 
					}
					else{ 
						children[1].insertarDato(p); 
					}
				} else{
					if(coorZ >= medZ){ 
						children[2].insertarDato(p); 
					}
					else { 
						children[0].insertarDato(p); 
					}
				}
			}


		}

		private void generarHijos(){

			isChildrenCreated=true;

			children=new Node[8];

			float minX=minAABB.getX();  float minY=minAABB.getY();
			float minZ=minAABB.getZ();
			float maxX=maxAABB.getX();  float maxY=maxAABB.getY();
			float maxZ=maxAABB.getZ();
			float medX=(maxX+minX)/2.0f; float medY=(maxY+minY)/2.0f;
			float medZ=(maxZ+minZ)/2.0f;

			children[6] = new Node(this,level+1,new Vertex(medX,maxY,maxZ),new Vertex(minX,medY,medZ),maxObjects,maxLevel,limitLevel);
			children[4] = new Node(this,level+1,new Vertex(medX,maxY,medZ),new Vertex(minX,medY,minZ),maxObjects,maxLevel,limitLevel);
			children[5] = new Node(this,level+1,new Vertex(maxX,maxY,medZ),new Vertex(medX,medY,minZ),maxObjects,maxLevel,limitLevel);
			children[7] = new Node(this,level+1,new Vertex(maxX,maxY,maxZ),new Vertex(medX,medY,medZ),maxObjects,maxLevel,limitLevel);
			children[2] = new Node(this,level+1,new Vertex(medX,medY,maxZ),new Vertex(minX,minY,medZ),maxObjects,maxLevel,limitLevel);
			children[0] = new Node(this,level+1,new Vertex(medX,medY,medZ),new Vertex(minX,minY,minZ),maxObjects,maxLevel,limitLevel);
			children[1] = new Node(this,level+1,new Vertex(maxX,medY,medZ),new Vertex(medX,minY,minZ),maxObjects,maxLevel,limitLevel);
			children[3] = new Node(this,level+1,new Vertex(maxX,medY,maxZ),new Vertex(medX,minY,medZ),maxObjects,maxLevel,limitLevel);


		}


		public Node[] getChildren(){
			return (isChildrenCreated) ? children : null;
		}

		 
	}


	private int maxLevel;
	private int limitLevel;
	private Node root;
	private static bool creacionStatus;


	public Octree(){

		maxLevel=1;
		limitLevel = maxLevel + 1;
		root=new Node();        
		creacionStatus=true;

	}



	public Octree(List<Vertex> vertexList,int _maxLevel,int maxObjects,Vertex minAABB,Vertex maxAABB){

		maxLevel=_maxLevel;

		AABB aabb=new AABB(minAABB,maxAABB);

		creacionStatus=true;

		/*
		 * En caso de haber muchos elementos en un nodo, el limite se establece como el nivel 
         * maximo establecido por el usuario +2
		*/
		limitLevel = maxLevel + 2;


		root=new Node(null,0,aabb.getMaxAABB(),aabb.getMinAABB(),maxObjects,maxLevel,limitLevel);

		foreach(Vertex vertexAux in vertexList){

			root.insertarDato(vertexAux); 

		}

	}

	public int getLimite(){
		return maxLevel;
	}

	//Recorre todos los nodos del octree guardando sus AABB
	private void busquedaNodosHoja(Node node,ref List<AABB> aabbList){

		Node [] children=node.getChildren();

		if(children!=null){

			busquedaNodosHoja(children[0], ref aabbList);
			busquedaNodosHoja(children[1], ref aabbList);
			busquedaNodosHoja(children[2], ref aabbList);
			busquedaNodosHoja(children[3], ref aabbList);        
			busquedaNodosHoja(children[4], ref aabbList);        
			busquedaNodosHoja(children[5], ref aabbList);
			busquedaNodosHoja(children[6], ref aabbList);
			busquedaNodosHoja(children[7], ref aabbList);
		}

		aabbList.Add(node.getAABB());

	}


	public List<AABB> getAABB(){

		List<AABB> listaAABB=new List<AABB>();

		busquedaNodosHoja(root,ref listaAABB);

		return listaAABB;

	}

	/*
        En caso de haber superado el numero maximo de vertices por nodo y
        el limite de expansion,se considera que el octree es correcto
        pero no es optimo
    */
	public bool isOptimus(){
		return creacionStatus;
	}


}
