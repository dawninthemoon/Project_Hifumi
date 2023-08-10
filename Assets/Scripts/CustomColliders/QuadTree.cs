using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CustomPhysics;

public interface IQuadTreeObject {
    Rectangle GetBounds();
}

public class QuadTree<T> where T : IQuadTreeObject {
	private int MAX_OBJECTS = 1;
	private int MAX_LEVELS = 3;
	
	private int _level;
	private List<T> _objects;
	private Rectangle _bounds;
	private QuadTree<T>[] _nodes;

	public QuadTree (int level, Rectangle bounds) {
		_level = level;
		_objects = new List<T>();
		_bounds = bounds;
		_nodes = new QuadTree<T>[4];
	}

	public void Clear() {
		_objects.Clear();
		
		for(int i  = 0; i < _nodes.Length; ++i) {
			if(_nodes[i] != null) {
				_nodes[i].Clear();
				_nodes[i] = null;
			}
		}
	}
	
	private void Split() {
		int subWidth = Mathf.FloorToInt(_bounds.width * 0.5f);
		int subHeight = Mathf.FloorToInt(_bounds.height * 0.5f);
		int x = (int)_bounds.position.x;
		int y = (int)_bounds.position.x;
		
		_nodes[0] = new QuadTree<T>(_level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
		_nodes[1] = new QuadTree<T>(_level + 1, new Rectangle(x, y, subWidth, subHeight));
		_nodes[2] = new QuadTree<T>(_level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
		_nodes[3] = new QuadTree<T>(_level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
	}
	
	private List<int> GetIndexes(Rectangle rect) {
	    List<int> indexes = new List<int>();
	
	    double verticalMidpoint = _bounds.position.x + (_bounds.width / 2);
	    double horizontalMidpoint = _bounds.position.y + (_bounds.height / 2);
	
		bool topQuadrant = rect.position.y >= horizontalMidpoint;
		bool bottomQuadrant = (rect.position.y - rect.height) <= horizontalMidpoint;
		bool topAndBottomQuadrant = rect.position.y + rect.height + 1 >= horizontalMidpoint && rect.position.y + 1 <= horizontalMidpoint;
		
		if (topAndBottomQuadrant) {
			topQuadrant = false;
			bottomQuadrant = false;
		}
	
		if(rect.position.x + rect.width + 1 >= verticalMidpoint && rect.position.x -1 <= verticalMidpoint) {
			if (topQuadrant) {
				indexes.Add(2);
				indexes.Add(3);
			}
			else if (bottomQuadrant) {
				indexes.Add(0);
				indexes.Add(1);
			}
			else if (topAndBottomQuadrant) {
				indexes.Add(0);
				indexes.Add(1);
				indexes.Add(2);
				indexes.Add(3);
			}
		}
		
		else if (rect.position.x + 1 >= verticalMidpoint) {
			if (topQuadrant) {
				indexes.Add(3);
			}
			else if (bottomQuadrant) {
				indexes.Add(0);
			}
			else if (topAndBottomQuadrant) {
				indexes.Add(3);
				indexes.Add(0);
			}
		}

		else if (rect.position.x - rect.width <= verticalMidpoint) {
			if (topQuadrant) {
				indexes.Add(2);
			}
			else if (bottomQuadrant) {
				indexes.Add(1);
			}
			else if (topAndBottomQuadrant) {
				indexes.Add(2);
				indexes.Add(1);
			}
		}
		else {
			indexes.Add(-1);
		}
	
		return indexes;
	}
	
	public void Insert(T obj) {
		Rectangle bounds = obj.GetBounds();
			
		if (_nodes[0] != null) {
			List<int> indexes = GetIndexes(bounds);
			for(int i = 0; i < indexes.Count; ++i) {
				int index = indexes[i];
				if (index != -1) {
					_nodes[index].Insert(obj);
					return;
				}
			}

		}
		
		_objects.Add(obj);
		
		if (_objects.Count > MAX_OBJECTS && _level < MAX_LEVELS) {
			if (_nodes[0] == null) {
				Split();
			}
			
			int i = 0;
			while (i < _objects.Count) {
				T quadTreeObject = _objects[i];
				Rectangle rect = quadTreeObject.GetBounds();
				List<int> indexes = GetIndexes(rect);
				for(int ii = 0; ii < indexes.Count; ii++)
				{
					int index = indexes[ii];
					if (index != -1)
					{
						_nodes[index].Insert(quadTreeObject);
						_objects.Remove(quadTreeObject);
					}
					else
					{
						i++;
					}
				}
			}
		}
	}
	
	public List<T> GetObjects(List<T> objectList, Rectangle rect) {
		return Retrieve(objectList, rect);
	}
	
	private List<T> Retrieve(List<T> objectList, Rectangle rect) {
		List<int> indexes = GetIndexes(rect);
		for (int i = 0; i < indexes.Count; ++i) {
			int index = indexes[i];
			if ((index != -1) && (_nodes[0] != null)) {
				_nodes[index].Retrieve(objectList, rect);
			}	
			objectList.AddRange(_objects);
		}
		
		return objectList;
	}
}