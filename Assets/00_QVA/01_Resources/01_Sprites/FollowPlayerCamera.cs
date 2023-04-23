using UnityEngine;

/// <summary>
/// Playerを追跡するCamera
/// </summary>
public class FollowPlayerCamera : MonoBehaviour
{
    /// <summary>
    /// Cameraが、追跡する対象 (Player)
    /// </summary>
    [SerializeField]
    public Transform target;
    /// <summary>
    /// 対象から離れた距離
    /// </summary>
    public Vector3 offset;
    
    /// <summary>
    /// 対象位置に、Camera移動
    /// </summary>
    private void LateUpdate() => transform.position = target.position + offset;
}
