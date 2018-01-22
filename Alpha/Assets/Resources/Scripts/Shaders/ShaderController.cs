using UnityEngine;
using System.Collections;
using EventHandler = Singleton<EventManager>;
using MaterialMan = Singleton<MaterialManager>;

public class ShaderController : MonoBehaviour
{
    /// <summary>
    /// Reference to this objects Mesh Renderer, if they have one.
    /// </summary>
    MeshRenderer _meshRenderer;

    /// <summary>
    /// Reference to this objects Plane Controller, if they have one.
    /// </summary>
    PlaneController _planeController;

    /// <summary>
    /// Bounds of the world
    /// </summary>
    Collider _worldBounds;

    /// <summary>
    /// If objects reach within this time of each other they will collide.
    /// </summary>
    float _collisionBufferTime = 2f;

    /// <summary>
    /// This objects name + "Gray"
    /// </summary>
    string _grayMaterialName;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _planeController = GetComponent<PlaneController>();
        _worldBounds = GameObject.Find("Scene").GetComponent<BoxCollider>();
        EventHandler.Instance.AddListener("ChangeShader", CheckObjectsPriority);
        EventHandler.Instance.AddListener(StringHandler.Events.RevertToDefaultShader, DefaultShader);

        if (CompareTag("Plane"))
        {
            _grayMaterialName = transform.parent.name + "Gray";
        }
        else
        {
            _grayMaterialName = gameObject.tag + "Gray";
        }
    }

    void CheckObjectsPriority(params object[] args)
    {
        PlaneController _controller = args[0] as PlaneController;

        if (_planeController == _controller) { return; }
        if (!_planeController && !gameObject.CompareTag("EntryGate") && !gameObject.CompareTag("ExitGate") && !gameObject.CompareTag("Path"))
        {
            for (int i = 0; i < _meshRenderer.materials.Length; ++i)
            {
                _meshRenderer.materials[i].shader = Shader.Find("Custom/Gray");
            }

            return;
        }

        if (_planeController)
        {
            if (CheckPlanePriority(_controller))
            {
                return;
            }
            
            if (_planeController.Path.name == _controller.Path.name)
            {
                return;
            }

            _meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(_grayMaterialName);
            return;
        }

        switch (gameObject.tag)
        {
            case "EntryGate":
                if (_controller.Path.startPosition.name != name)
                {
                    _meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(_grayMaterialName);
                    return;
                }
                break;
            case "ExitGate":
                if (_controller.Path.endPosition.name != name)
                {
                    _meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(_grayMaterialName);
                    return;
                }
                break;
            case "Path":
                if (_controller.Path.name != name)
                {
                    _meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(_grayMaterialName);
                    return;
                }
                break;
        }
    }

    void DefaultShader(params object[] args)
    {
        PlaneController currentSelected = args[0] as PlaneController;

        if (_planeController)
        {
            if (_planeController != currentSelected)
            {
                _meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(transform.parent.name);
                return;
            }
        }
        else if (!gameObject.CompareTag("Untagged"))
        {
            _meshRenderer.sharedMaterial = MaterialMan.Instance.GetMaterial(gameObject.tag);
        }
    }

    bool CheckPlanePriority(PlaneController _currentSelected)
    {
        Vector3 A = transform.position;
        Vector3 B = _currentSelected.transform.position;

        Vector3 a = transform.forward;
        Vector3 b = _currentSelected.transform.forward;

        float dotProd = Vector3.Dot(a, b);

        if (dotProd < 0)
        {
            return false;
        }

        Vector3 n = Vector3.Cross(a, b);
        Vector3 u = Vector3.Cross(n, A - B) / Vector3.Dot(n, n);

        Vector3 BB = B - b * Vector3.Dot(a, u);
        Vector3 intersectionDir = (BB - transform.position).normalized;
        dotProd = Vector3.Dot(transform.forward, intersectionDir);

        if (dotProd < 0)
        {
            return false;
        }

        if (_worldBounds.bounds.Contains(BB))
        {
            if (CheckTimeTillCollision(_currentSelected, BB))
            {
                return true;
            }
        }
        return false;
    }

    bool CheckTimeTillCollision(PlaneController _currentlySelected, Vector3 _collisionPoint)
    {
        float planeToCollisionMagnitude = (_collisionPoint - transform.position).magnitude;
        float planeTimeTillCollision = planeToCollisionMagnitude / _planeController.Speed;
        float currentSelectedPlaneMagnitude = (_collisionPoint - _currentlySelected.transform.position).magnitude;
        float currentSelectedTimeTillCollision = (currentSelectedPlaneMagnitude / _currentlySelected.Speed);
        if (Mathf.Abs((currentSelectedTimeTillCollision) - (planeTimeTillCollision)) < _collisionBufferTime)
        {
            return true;
        }
        return false;
    }

    float GlowPercentage()
    {
        float k = 0f;
        return k;
    }

}