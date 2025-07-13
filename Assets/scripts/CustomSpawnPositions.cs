using Meta.XR.Util;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomSpawnPositions : MonoBehaviour
{
      public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

        [SerializeField, Tooltip("Prefab to be placed into the scene, or object in the scene to be moved around.")]
        public GameObject SpawnObject;

        [SerializeField, Tooltip("Number of SpawnObject(s) to place into the scene per room.")]
        public int SpawnAmount = 8;

        [SerializeField, Tooltip("Optional debug material for overlap check visualization.")]
        public Material DebugBoxMaterial;

        [SerializeField, Tooltip("Enable to visualize the overlap check boxes in the scene.")]
        public bool ShowDebugBoxes = false;

        [SerializeField, Tooltip("Maximum number of times to attempt spawning/moving an object before giving up.")]
        public int MaxIterations = 1000;


        public enum SpawnLocation
        {
            Floating,
            AnySurface,
            VerticalSurfaces,
            OnTopOfSurfaces,
            HangingDown
        }

        [SerializeField, Tooltip("Attach content to scene surfaces.")]
        public SpawnLocation SpawnLocations = SpawnLocation.Floating;

        [SerializeField, Tooltip("Filter which anchor labels should be included.")]
        public MRUKAnchor.SceneLabels Labels = ~(MRUKAnchor.SceneLabels)0;

        [SerializeField, Tooltip("If enabled, check for overlap with physics colliders.")]
        public bool CheckOverlaps = true;

        [SerializeField, Tooltip("Required free space for the object (Set negative to auto-detect).")]
        public float OverrideBounds = -1;

        [SerializeField, Tooltip("Layer(s) for the physics bounding box checks.")]
        public LayerMask LayerMask = -1;

        [SerializeField, Tooltip("The clearance distance in front of the surface.")]
        public float SurfaceClearanceDistance = 0.1f;

        public void StartSpawn()
        {
            foreach (var room in MRUK.Instance.Rooms)
            {
                StartSpawn(room);
            }
        }

        /// <summary>
        /// Starts the spawning process for a specific room.
        /// This version uses CheckCapsule instead of CheckBox and has extra debug logs.
        /// </summary>
        public void StartSpawn(MRUKRoom room)
        {
            Debug.Log("[CustomSpawnPositions] Starting spawn in room.");

            var prefabBounds = Utilities.GetPrefabBounds(SpawnObject);
            float minRadius = 0.0f;
            const float clearanceDistance = 0.01f;
            float baseOffset = -prefabBounds?.min.y ?? 0.0f;
            float centerOffset = prefabBounds?.center.y ?? 0.0f;
            Bounds adjustedBounds = new();

            if (prefabBounds.HasValue)
            {
                minRadius = Mathf.Min(-prefabBounds.Value.min.x, -prefabBounds.Value.min.z, prefabBounds.Value.max.x, prefabBounds.Value.max.z);
                if (minRadius < 0f) minRadius = 0f;

                var min = prefabBounds.Value.min;
                var max = prefabBounds.Value.max;
                min.y += clearanceDistance;
                if (max.y < min.y) max.y = min.y;

                adjustedBounds.SetMinMax(min, max);

                if (OverrideBounds > 0)
                {
                    Vector3 center = new Vector3(0f, clearanceDistance, 0f);
                    Vector3 size = new Vector3(OverrideBounds * 2f, clearanceDistance * 2f, OverrideBounds * 2f);
                    adjustedBounds = new Bounds(center, size);
                }
            }

            for (int i = 0; i < SpawnAmount; ++i)
            {
                bool foundValidSpawnPosition = false;
                for (int j = 0; j < MaxIterations; ++j)
                {
                    Vector3 spawnPosition = Vector3.zero;
                    Vector3 spawnNormal = Vector3.up;

                    if (SpawnLocations == SpawnLocation.Floating)
                    {
                        var randomPos = room.GenerateRandomPositionInRoom(minRadius, true);
                        if (!randomPos.HasValue) break;
                        spawnPosition = randomPos.Value;
                    }
                    else
                    {
                        MRUK.SurfaceType surfaceType = 0;
                        switch (SpawnLocations)
                        {
                            case SpawnLocation.AnySurface:
                                surfaceType |= MRUK.SurfaceType.FACING_UP | MRUK.SurfaceType.VERTICAL | MRUK.SurfaceType.FACING_DOWN;
                                break;
                            case SpawnLocation.VerticalSurfaces:
                                surfaceType |= MRUK.SurfaceType.VERTICAL;
                                break;
                            case SpawnLocation.OnTopOfSurfaces:
                                surfaceType |= MRUK.SurfaceType.FACING_UP;
                                break;
                            case SpawnLocation.HangingDown:
                                surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                                break;
                        }

                        if (room.GenerateRandomPositionOnSurface(surfaceType, minRadius, new LabelFilter(Labels), out var pos, out var normal))
                        {
                            spawnPosition = pos + normal * baseOffset;
                            spawnNormal = normal;
                            var center = spawnPosition + normal * centerOffset;

                            if (!room.IsPositionInRoom(center))
                            {
                                Debug.LogWarning("[CustomSpawnPositions] Position outside room bounds, skipping.");
                                continue;
                            }

                            if (room.IsPositionInSceneVolume(center))
                            {
                                Debug.LogWarning("[CustomSpawnPositions] Position inside scene volume, skipping.");
                                continue;
                            }

                            if (room.Raycast(new Ray(pos, normal), SurfaceClearanceDistance, out _))
                            {
                                Debug.LogWarning("[CustomSpawnPositions] Raycast hit near surface, skipping.");
                                continue;
                            }
                        }
                    }

                    Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);

                if (CheckOverlaps)
                {
                    // Temporäre, unsichtbare Instanz an der Zielposition erzeugen
                    var testInstance = Instantiate(SpawnObject);
                    testInstance.transform.position = spawnPosition;
                    testInstance.transform.rotation = spawnRotation;
                    testInstance.SetActive(false); // Kein Renderer sichtbar

                    // Collider suchen (egal ob Box, Mesh, Capsule, ...)
                    var collider = testInstance.GetComponentInChildren<Collider>();
                    if (collider == null)
                    {
                        Debug.LogError("[CustomSpawnPositions] ❌ Kein Collider im SpawnObject gefunden!");
                        Destroy(testInstance);
                        continue;
                    }
                    collider.enabled = true;

                    // Physics.OverlapBox mit Collider.bounds
                    Vector3 center = collider.bounds.center;
                    Vector3 extents = collider.bounds.extents;
                    Quaternion rotation = collider.transform.rotation;

                    Debug.Log($"[CustomSpawnPositions] OverlapBox Test: Center={center}, Extents={extents}, Rotation={rotation.eulerAngles}");
                if (ShowDebugBoxes && DebugBoxMaterial != null)
                {
                    var debugBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    debugBox.transform.position = center;
                    debugBox.transform.rotation = rotation;
                    debugBox.transform.localScale = extents * 2f; // BoxCollider size = extents * 2
                    debugBox.GetComponent<Renderer>().material = DebugBoxMaterial;

                    // Optional: Make it easier to clean up
                    Destroy(debugBox.GetComponent<Collider>());
                    Destroy(debugBox, 10f); // Auto-destroy after 10s
                }

                    if (Physics.CheckBox(center, extents, rotation, LayerMask, QueryTriggerInteraction.Ignore))
                    {
                        Debug.LogWarning("[CustomSpawnPositions] Overlap detected with existing objects, skipping this spawn position.");
                        Destroy(testInstance);
                        continue;
                    }

                    // Kein Overlap → Success!
                    Destroy(testInstance);
                    Debug.Log("[CustomSpawnPositions] No overlap detected. Spawning here!");
                }
                    foundValidSpawnPosition = true;

                    if (SpawnObject.gameObject.scene.path == null)
                    {
                        Instantiate(SpawnObject, spawnPosition, spawnRotation, transform);
                        Debug.Log($"[CustomSpawnPositions] Instantiated new object at {spawnPosition}");
                    }
                    else
                    {
                        SpawnObject.transform.position = spawnPosition;
                        SpawnObject.transform.rotation = spawnRotation;
                        Debug.Log($"[CustomSpawnPositions] Moved existing object to {spawnPosition}");
                        return;
                    }

                    break;
                }

                if (!foundValidSpawnPosition)
                {
                    Debug.LogWarning($"[CustomSpawnPositions] Failed to find valid spawn position after {MaxIterations} attempts. Only spawned {i} prefabs instead of {SpawnAmount}.");
                    break;
                }
            }
        }
    }

