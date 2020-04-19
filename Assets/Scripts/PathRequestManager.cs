using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    public static PathRequestManager instance;

    public Pathfinding pathfinding;

    private Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
    private PathRequest _currentPathRequest;
    private bool _isProcessingPath = false;

    struct PathRequest {
        public Vector2 start;
        public Vector2 end;
        public GameObject gameObject;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 start, Vector2 end, GameObject gameObject, Action<Vector2[], bool> callback) {
            this.start = start;
            this.end = end;
            this.gameObject = gameObject;
            this.callback = callback;
        }
    }

    public static void RequestPath(Vector2 start, Vector2 end, GameObject gameObject, Action<Vector2[], bool> callback) {
        PathRequest request = new PathRequest(start, end, gameObject, callback);
        instance._pathRequestQueue.Enqueue(request);
        instance.TryProcessNext();
    }

    public void Awake() {
        instance = this;
    }

    public void FinishedProcessingPath(Vector2[] path, bool success) {
        if (_currentPathRequest.gameObject != null) _currentPathRequest.callback(path, success);
        _isProcessingPath = false;
        TryProcessNext();
    }

    private void TryProcessNext() {
        if (!_isProcessingPath && _pathRequestQueue.Count > 0) {
            _currentPathRequest = _pathRequestQueue.Dequeue();
            _isProcessingPath = true;

            pathfinding.StartFindPath(_currentPathRequest.start, _currentPathRequest.end);
        }
    }
}
