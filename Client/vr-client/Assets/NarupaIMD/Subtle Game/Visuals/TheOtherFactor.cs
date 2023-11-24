using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Burst;
using System.Collections;
using Oculus.Interaction;
using UnityEngine.ParticleSystemJobs;
using System;
using System.Threading.Tasks;

public class TheOtherFactor : MonoBehaviour
{
    [HideInInspector]
    public List<Vector3> EmitPositions = new List<Vector3>(); 
    #region Instantiator Variables
    private GameObject hands;
    public float ParticleSize = .042f;
    #region Hand References for Joints (not in use)
    private HandVisual LeftHandVisual;
    private HandVisual RightHandVisual;
    #endregion

    public int ParticlesPerHand = 2500;
    [HideInInspector]
    public List<Transform> Joints = new List<Transform>();
    private List<Transform> LeftHandJoints = new List<Transform>();
    private List<Transform> RightHandJoints = new List<Transform>();

    private SkinnedMeshRenderer leftHandMesh;
    private SkinnedMeshRenderer rightHandMesh;

    private List<Vector3> LeftHandRelativePositions = new List<Vector3>();
    private List<Vector3> RightHandRelativePositions = new List<Vector3>();
    private List<int> LeftHandJointIndices = new List<int>();
    private List<int> RightHandJointIndices = new List<int>();
    #endregion
    #region Mediator Variables
    [System.Serializable]
    public class JointData
    {
        public int jointIndex;
        public string jointName;
        public Vector3 jointWorldPosition; 
        public List<Vector3> relativePositions = new List<Vector3>();
    }
    private List<JointData> AllJointsData = new List<JointData>();
    public bool useDefaultPath = true;
    public List<string> LeftHandParticlePath = new List<string>();
    public List<string> RightHandParticlePath = new List<string>();

    private List<int> outputLeftHandJointIndices = new List<int>();
    private List<Vector3> outputLeftHandRelativePositions = new List<Vector3>();
    private List<int> outputRightHandJointIndices = new List<int>();
    private List<Vector3> outputRightHandRelativePositions = new List<Vector3>();
    private bool firstActivation = true;
    #endregion
    #region Engine Variables
    private bool EngineOn = false;

    #region Gauss
    [Header("Gauss")]
    public bool OscGauss = true;
    public float Gauss = 0.1252471f; 
    public float GaussMin = 0.125f; 
    public float GaussMax = 0.15f; 
    private float GaussOscSpeed = 1f; 

    [Header("Gauss Noise")]
    public float GaussNoiseMin = .2f;
    public float GaussNoiseMax = 1f;

    #region Gauss Noise Oscillation
    [Header("Gauss Noise Min Osc")]
    public bool OscGaussNoiseMin = true;
    public float GaussNoiseMinMin = .175f;
    public float GaussNoiseMinMax = .225f;
    private float GaussNoiseMinOscSpeed = 1f;

    [Header("Gauss Noise Max Osc")]
    public bool OscGaussNoiseMax = false;
    public float GaussNoiseMaxMin = .875f;
    public float GaussNoiseMaxMax = 1.05f;
    private float GaussNoiseMaxOscSpeed = 1f;

    [Header("Gauss Noise MaxMin Osc")]
    public bool OscGaussNoiseMaxMin = true;
    public float GaussNoiseMaxMinMin = .775f;
    public float GaussNoiseMaxMinMax = .825f;
    private float GaussNoiseMaxMinOscSpeed = 1f;
    #endregion
    #endregion

    #region Speed Limit
    [Header("Speed Limit")]
    public bool OscSpeedLimit = false;
    public float SpeedLimit = 100f;
    public float SpeedLimitMin = 0.75f;
    public float SpeedLimitMax = 85f;
    public float SpeedLimitOscSpeed = 100;
    #endregion

    #region Position Noise
    [Header("Position Noise")]
    public float PosNoiseMin = 0.4411342f;
    public float PosNoiseMax = .5f;

    [Header("Position Noise Min Osc")]
    public bool OscPosNoiseMin = true;
    public float PosNoiseMinMin = .45f;
    public float PosNoiseMinMax = .55f;
    private float PosNoiseMinOscSpeed = 1f;
    #endregion

    #region Oscillation Speed
    [Header("Oscillation Speed")]
    public float OscSpeedMin = .5f;
    public float OscSpeedMax = 1f;
    #endregion

    #region The Other Factor
    [Header("The Other Factor")]
    public float TheOtherFactorMin = .85f;
    public float TheOtherFactorMax = .99f;
    #endregion

    #region Index Step Size
    [Header("Index Step Size")]
    public int IndexStepSizeMin = 1;
    public int IndexStepSizeMax = 3;

    [Header("Index Step Size Min Osc")]
    public bool OscIndexStepSizeMax = true;
    public int IndexStepSizeMaxMin = 2;
    public int IndexStepSizeMaxMax = 4;
    private float IndexStepSizeMaxOscSpeed = 1f;
    #endregion

    #region Color
    public bool OscColor = true;
    public Color RGB = Color.white;
    public Color MinRGB = Color.white;
    public Color MaxRGB = Color.white;
    private Color ColorOscSpeeds = Color.white;

    public bool OscColorMin = true;
    public Color MinRGBMin = Color.white;
    public Color MinRGBMax = Color.white;
    private Color MinRGBOscSpeeds = Color.white;

    public bool OscColorMax = true;
    public Color MaxRGBMin = Color.white;
    public Color MaxRGBMax = Color.white;
    private Color MaxRGBOscSpeeds = Color.white;
    #endregion

    #region Job Variable Communicators
    private NativeArray<float> nativeAttractionNoise;
    private NativeArray<float> nativePositionNoise;
    private NativeArray<float> nativeTheOtherFactors;
    private NativeArray<int> nativeIndexStepSize;

    private NativeArray<Vector3> nativeJointPositions;
    private NativeArray<quaternion> nativeJointRotations;

    public NativeArray<Vector3> nativeRelativePositionsLeft;
    public NativeArray<Vector3> nativeRelativePositionsRight;
    public NativeArray<int> nativeJointIndicesLeft;
    public NativeArray<int> nativeJointIndicesRight;
    [HideInInspector]
    public int maxIndexLeftHand;
    [HideInInspector]
    public int maxIndexRightHand;

    private NativeArray<Vector3> nativePreviousVelocities;
    private NativeArray<Vector3> nativePreviousPositions;

    private int currentTrailIndexOffset = 0;
    private ParticleSystem particleSystem;
    private ParticleAttractionJob particleAttractionJob;
    private JobHandle particleAttractionJobHandle;
    #endregion
    #endregion

    private void Start()
    {
        #region Find References
        #region Particle System
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            particleSystem = gameObject.AddComponent<ParticleSystem>();
            Debug.Log("ParticleSystem component was not found and has been added.");
        }

        ParticleSystem.MainModule mainModule = particleSystem.main;
        mainModule.maxParticles = 1000000;
        mainModule.startColor = Color.black;
        mainModule.playOnAwake = false;

        // Get the ParticleSystemRenderer component
        ParticleSystemRenderer particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();

        // Check if the current material is null
        if (particleRenderer.material == null)
        {
            // Create a new material with the Mobile/Particles/Additive shader
            Material newParticleMaterial = new Material(Shader.Find("Mobile/Particles/Additive"));

            // Optionally set properties of the new material here
            // For example, setting a color
            newParticleMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f, 1f)); // Set to white by default

            // Assign the new material to the ParticleSystemRenderer
            particleRenderer.material = newParticleMaterial;
        }
        #endregion
        #region Hands
        LeftHandVisual = GameObject.Find("LeftHandVisual")?.GetComponent<HandVisual>();
        if (LeftHandVisual == null)
        {
            Debug.LogError("LeftHandVisual GameObject or HandVisual component not found.");
        }

        RightHandVisual = GameObject.Find("RightHandVisual")?.GetComponent<HandVisual>();
        if (RightHandVisual == null)
        {
            Debug.LogError("RightHandVisual GameObject or HandVisual component not found.");
        }

        hands = GameObject.Find("Hands");
        if (hands == null)
        {
            Debug.LogError("Hands GameObject not found.");
        }

        leftHandMesh = GameObject.Find("l_handMeshNode")?.GetComponent<SkinnedMeshRenderer>();
        if (leftHandMesh == null)
        {
            Debug.LogError("l_handMeshNode GameObject or SkinnedMeshRenderer component not found.");
        }

        rightHandMesh = GameObject.Find("r_handMeshNode")?.GetComponent<SkinnedMeshRenderer>();
        if (rightHandMesh == null)
        {
            Debug.LogError("r_handMeshNode GameObject or SkinnedMeshRenderer component not found.");
        }
        #endregion
        #endregion
    }
    public async void StartTheOtherFactor()
    {
        gameObject.SetActive(true);
        await InitializeParticlesCoroutine();
    }
    public void StopTheOtherFactor()
    {
        EngineOn = false;
        //leftHandMesh.gameObject.SetActive(true);
        //rightHandMesh.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
    #region Log Color Values to Console To Safe in Script
    public void LogColorValuesToConsole()
    {

        Debug.Log("MinRGB: " + ColorToString(MinRGB));
        Debug.Log("MaxRGB: " + ColorToString(MaxRGB));

        Debug.Log("MinRGBMin: " + ColorToString(MinRGBMin));
        Debug.Log("MinRGBMax: " + ColorToString(MinRGBMax));

        Debug.Log("MaxRGBMin: " + ColorToString(MaxRGBMin));
        Debug.Log("MaxRGBMax: " + ColorToString(MaxRGBMax));
    }
    private string ColorToString(Color color)
    {
        return $"({color.r}, {color.g}, {color.b}, {color.a})";
    }
    #endregion

    public async Task InitializeParticlesCoroutine()
    {
        #region Disable Hand Tracking for Initialization
        // Disable hand tracking at the start so hands wont move while we instantiate mesh points.
        hands.SetActive(false);
        #endregion       
        
        FetchJointData();
        await Task.Yield();

        InitializeMeshPoints();
        await Task.Yield();

        if (useDefaultPath) CreateDefaultPath();      
        await Task.Yield();

        ProcessInputData();
        await Task.Yield();

        GenerateParticlePathData();
        await Task.Yield();

        EmitParticles();
        await Task.Yield();

        InitializeNativeArrays();
        await Task.Yield();

        AssignJointIndicesAndRelativePositions();
        await Task.Yield();

        StartCoroutine(StartEngine());
        await Task.Yield();

        hands.SetActive(true);
        leftHandMesh.gameObject.SetActive(false);
        rightHandMesh.gameObject.SetActive(false);
    }
    #region Fetch Joint Data
    public void FetchJointData()
    {
        InitializeHandJointsFromHandVisual(LeftHandVisual, "left");
        InitializeHandJointsFromHandVisual(RightHandVisual, "right");
        #region Concatonate Joint Lists
        // Concatenate the individual joint lists to the original Joints list
        Joints.Clear();
        Joints.AddRange(LeftHandJoints);
        Joints.AddRange(RightHandJoints);
        #endregion
    }
    // Method to initialize hand joints from HandVisual
    public void InitializeHandJointsFromHandVisual(HandVisual handVisual, string handType)
    {
        IList<Transform> jointTransforms = handVisual.Joints;
        HashSet<string> excludedJoints = new HashSet<string> { "b_l_wrist", "b_r_wrist", "b_l_forearm_stub", "b_r_forearm_stub" };

        if (handType == "right")
        {
            RightHandJoints.Clear();

            foreach (Transform joint in jointTransforms)
            {
                if (!excludedJoints.Contains(joint.name))
                {
                    RightHandJoints.Add(joint);
                }
            }
        }
        else if (handType == "left")
        {
            LeftHandJoints.Clear();

            foreach (Transform joint in jointTransforms)
            {
                if (!excludedJoints.Contains(joint.name))
                {
                    LeftHandJoints.Add(joint);
                }
            }
        }
    }
    #endregion
    #region Initialize Mesh Points
    public void InitializeMeshPoints()
    {
        var leftHandPoints = InstantiateMeshPoints(leftHandMesh);
        var rightHandPoints = InstantiateMeshPoints(rightHandMesh);

        LeftHandRelativePositions.Clear();
        RightHandRelativePositions.Clear();
        LeftHandJointIndices.Clear();
        RightHandJointIndices.Clear();

        InitializeRelativePositions(leftHandPoints, LeftHandJoints, 0, LeftHandRelativePositions, LeftHandJointIndices);
        InitializeRelativePositions(rightHandPoints, RightHandJoints, LeftHandJoints.Count, RightHandRelativePositions, RightHandJointIndices);
    }
    private Vector3[] InstantiateMeshPoints(SkinnedMeshRenderer skinnedMeshRenderer)
    {
        var meshObject = skinnedMeshRenderer.gameObject.transform;
        var bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        var vertices = new NativeArray<Vector3>(bakedMesh.vertices, Allocator.TempJob);
        var triangles = new NativeArray<int>(bakedMesh.triangles, Allocator.TempJob);
        var points = new NativeArray<Vector3>(ParticlesPerHand, Allocator.TempJob);
        var barycentricCoords = new NativeArray<Vector3>(ParticlesPerHand, Allocator.TempJob);
        var triangleIndices = new NativeArray<int>(ParticlesPerHand, Allocator.TempJob);

        var job = new InstantiateMeshPointsJob
        {
            vertices = vertices,
            triangles = triangles,
            points = points,
            barycentricCoords = barycentricCoords,
            triangleIndices = triangleIndices,
            random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue)) // Seed the random number generator
        };

        var jobHandle = job.Schedule(ParticlesPerHand, 64);
        jobHandle.Complete();

        var result = points.ToArray();

        // Transform the points to world space
        for (int i = 0; i < ParticlesPerHand; i++)
        {
            result[i] = meshObject.TransformPoint(result[i]);
        }

        vertices.Dispose();
        triangles.Dispose();
        points.Dispose();
        barycentricCoords.Dispose();
        triangleIndices.Dispose();

        return result;
    }
    private struct InstantiateMeshPointsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> vertices;
        [ReadOnly] public NativeArray<int> triangles;
        public NativeArray<Vector3> points;
        public NativeArray<Vector3> barycentricCoords;
        public NativeArray<int> triangleIndices;
        public Random random;

        public void Execute(int index)
        {
            // Generate a random barycentric coordinate
            float r1 = math.sqrt(random.NextFloat());
            float r2 = random.NextFloat();
            float u = 1 - r1;
            float v = r1 * (1 - r2);
            float w = r1 * r2;
            barycentricCoords[index] = new Vector3(u, v, w);
            Vector3 v1 = Vector3.zero;
            Vector3 v2 = Vector3.zero;
            Vector3 v3 = Vector3.zero;

            // Compute the areas of the triangles
            float[] areas = new float[triangles.Length / 3];
            float totalArea = 0;
            for (int i = 0; i < triangles.Length / 3; i++)
            {
                v1 = vertices[triangles[i * 3]];
                v2 = vertices[triangles[i * 3 + 1]];
                v3 = vertices[triangles[i * 3 + 2]];
                areas[i] = Vector3.Cross(v2 - v1, v3 - v1).magnitude / 2;
                totalArea += areas[i];
            }

            // Compute the size of each stratum
            float stratumSize = totalArea / points.Length;

            // Select a random triangle within the stratum
            float stratumStart = stratumSize * index;
            float stratumEnd = stratumStart + stratumSize;
            float cumulativeArea = 0;
            int triangleIndex = 0;
            for (; triangleIndex < areas.Length; triangleIndex++)
            {
                cumulativeArea += areas[triangleIndex];
                if (cumulativeArea >= stratumStart)
                {
                    break;
                }
            }
            triangleIndices[index] = triangleIndex * 3;

            Vector3 uvw = barycentricCoords[index];
            triangleIndex = triangleIndices[index];

            v1 = vertices[triangles[triangleIndex]];
            v2 = vertices[triangles[triangleIndex + 1]];
            v3 = vertices[triangles[triangleIndex + 2]];

            // Compute the point within the triangle
            points[index] = uvw.x * v1 + uvw.y * v2 + uvw.z * v3;
        }
    }
    private void InitializeRelativePositions(Vector3[] worldCoordinates, List<Transform> joints, int jointIndexOffset, List<Vector3> targetRelativePositions, List<int> targetJointIndices)
    {
        // Convert joint positions to a NativeArray.
        var jointPositions = new NativeArray<Vector3>(joints.Count, Allocator.TempJob);
        // Convert joint rotations to a NativeArray.
        var jointRotations = new NativeArray<quaternion>(joints.Count, Allocator.TempJob);
        for (int i = 0; i < joints.Count; i++)
        {
            jointPositions[i] = joints[i].position;
            jointRotations[i] = joints[i].rotation;
        }
        // Create a job to compute the closest joint for each point.
        var job = new GetClosestJointJob
        {
            jointPositions = jointPositions,
            jointRotations = jointRotations,
            points = new NativeArray<Vector3>(worldCoordinates, Allocator.TempJob),
            relativePositions = new NativeArray<Vector3>(worldCoordinates.Length, Allocator.TempJob),
            jointIndices = new NativeArray<int>(worldCoordinates.Length, Allocator.TempJob),
        };

        // Schedule and complete the job.
        var jobHandle = job.Schedule(worldCoordinates.Length, 64);
        jobHandle.Complete();

        for (int i = 0; i < worldCoordinates.Length; i++)
        {
            targetRelativePositions.Add(job.relativePositions[i]);
            targetJointIndices.Add(job.jointIndices[i] + jointIndexOffset);
        }

        // Dispose of the NativeArrays.
        jointPositions.Dispose();
        jointRotations.Dispose();
        job.points.Dispose();
        job.relativePositions.Dispose();
        job.jointIndices.Dispose();
    }
    [BurstCompile]
    public struct GetClosestJointJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> jointPositions;
        [ReadOnly] public NativeArray<quaternion> jointRotations;
        [ReadOnly] public NativeArray<Vector3> points;
        public NativeArray<Vector3> relativePositions;
        public NativeArray<int> jointIndices;

        public void Execute(int index)
        {
            var point = points[index];
            var closestJointIndex = 0;
            var closestDistance = Vector3.Distance(jointPositions[0], point);

            // Find the closest joint.
            for (int i = 1; i < jointPositions.Length; i++)
            {
                var distance = Vector3.Distance(jointPositions[i], point);
                if (distance < closestDistance)
                {
                    closestJointIndex = i;
                    closestDistance = distance;
                }
            }

            // Compute the relative position.
            var relativePosition = point - jointPositions[closestJointIndex];

            // Rotate the relative position by the inverse of the joint's rotation.
            relativePosition = math.mul(math.inverse(jointRotations[closestJointIndex]), relativePosition);

            // Store the relative position and joint index.
            relativePositions[index] = relativePosition;
            jointIndices[index] = closestJointIndex;
        }
    }
    #endregion
    #region Create Default Path
    public void CreateDefaultPath()
    {
        // Clearing existing data for both hands
        LeftHandParticlePath.Clear();
        RightHandParticlePath.Clear();

        // Iterating over the Joints list in MeshPointInstantiator
        foreach (Transform jointTransform in Joints)
        {
            string jointName = jointTransform.name;
            string simplifiedName = SimplifyJointName(jointName);  // Simplifying the joint name

            //LeftHandParticlePath.Add(simplifiedName);
            
            // Checking prefix of the simplified name to decide which hand it belongs to
            if (simplifiedName.StartsWith("L-"))
            {
                LeftHandParticlePath.Add(simplifiedName);
            }
            else if (simplifiedName.StartsWith("R-"))
            {
                RightHandParticlePath.Add(simplifiedName);
            }
            
        }
    }
    private string SimplifyJointName(string originalName)
    {
        // Convert to lowercase
        originalName = originalName.ToLowerInvariant();

        // Replace specific patterns with clearer names
        originalName = originalName.Replace("thumb", "Thumb")
                                  .Replace("index", "Index")
                                  .Replace("middle", "Middle")
                                  .Replace("ring", "Ring")
                                  .Replace("pinky", "Pinky")
                                  .Replace("forearm_stub", "Forearm")
                                  .Replace("finger_tip_marker", "Tip")
                                  .Replace("palm_center_marker", "Palm")
                                  .Replace("wrist", "Wrist");

        // Handle side specification
        if (originalName.StartsWith("b_l_"))
            originalName = "L-" + originalName.Substring(4);
        else if (originalName.StartsWith("b_r_"))
            originalName = "R-" + originalName.Substring(4);
        else if (originalName.StartsWith("l_"))
            originalName = "L-" + originalName.Substring(2);
        else if (originalName.StartsWith("r_"))
            originalName = "R-" + originalName.Substring(2);

        return originalName;
    }
    #endregion
    #region Process Input Data
    public void ProcessInputData()
    {
        // Clear existing data
        AllJointsData.Clear();

        // Create a temporary dictionary to store the joint data for efficient lookup
        Dictionary<int, JointData> jointDataDict = new Dictionary<int, JointData>();

        // Method to process joint data for a hand
        void ProcessHandData(List<int> jointIndices, List<Vector3> relativePositions)
        {
            for (int i = 0; i < jointIndices.Count; i++)
            {
                int jointIndex = jointIndices[i];
                Vector3 relativePos = relativePositions[i];

                if (!jointDataDict.ContainsKey(jointIndex))
                {
                    JointData newJointData = new JointData
                    {
                        jointIndex = jointIndex,
                        jointWorldPosition = Joints[jointIndex].position,
                        jointName = SimplifyJointName(Joints[jointIndex].name),
                        relativePositions = new List<Vector3> { relativePos }
                    };

                    jointDataDict[jointIndex] = newJointData;
                }
                else
                {
                    jointDataDict[jointIndex].relativePositions.Add(relativePos);
                }
            }
        }

        // Process data for left and right hands
        ProcessHandData(LeftHandJointIndices, LeftHandRelativePositions);
        ProcessHandData(RightHandJointIndices, RightHandRelativePositions);

        // Add the contents of the dictionary to the final list
        AllJointsData.AddRange(jointDataDict.Values);
    }
    #endregion
    #region Generate Particle Path Data
    public void GenerateParticlePathData()
    {
        // Clear existing output data for both hands
        outputLeftHandJointIndices.Clear();
        outputLeftHandRelativePositions.Clear();
        outputRightHandJointIndices.Clear();
        outputRightHandRelativePositions.Clear();

        if (firstActivation)
        {
            LeftHandParticlePath.AddRange(RightHandParticlePath);
            firstActivation = false;
        }

        // Helper function to populate and sort output data based on a given particle path and joint data
        void PopulateOutputData(List<string> particlePath, List<int> outputJointIndices, List<Vector3> outputRelativePos)
        {
            for (int pathIndex = 0; pathIndex < particlePath.Count; pathIndex++)
            {
                string jointName = particlePath[pathIndex];
                // Find the JointData corresponding to the current joint name
                JointData jointData = AllJointsData.Find(jd => jd.jointName == jointName);

                if (jointData != null)
                {
                    // Temporary list to store and sort the relative positions
                    List<Vector3> sortedRelativePositions = new List<Vector3>(jointData.relativePositions);

                    // Determine the next joint index for distance comparison
                    int nextJointIndex = (pathIndex < particlePath.Count - 1) ?
                        AllJointsData.Find(jd => jd.jointName == particlePath[pathIndex + 1]).jointIndex :
                        AllJointsData.Find(jd => jd.jointName == particlePath[0]).jointIndex;

                    // Sort based on distance to the next joint's position
                    sortedRelativePositions.Sort((pos1, pos2) =>
                    {
                        float distance1 = Vector3.Distance(Joints[nextJointIndex].position, jointData.jointWorldPosition + pos1);
                        float distance2 = Vector3.Distance(Joints[nextJointIndex].position, jointData.jointWorldPosition + pos2);
                        return distance1.CompareTo(distance2);
                    });

                    // Add joint index for each relative position
                    foreach (var pos in sortedRelativePositions)
                    {
                        outputJointIndices.Add(jointData.jointIndex);
                    }

                    // Add sorted relative positions
                    outputRelativePos.AddRange(sortedRelativePositions);
                }
            }
        }

        // Populate output data for both hands
        PopulateOutputData(LeftHandParticlePath, outputLeftHandJointIndices, outputLeftHandRelativePositions);
        PopulateOutputData(RightHandParticlePath, outputRightHandJointIndices, outputRightHandRelativePositions);
    }
    #endregion
    #region Emit Particles
    private void EmitParticles()
    {
        int totalParticles = ParticlesPerHand * 2;
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
        {
            startSize = ParticleSize,
            startLifetime = 3600f, // 1 hour
        };

        if (EmitPositions.Count > 0)
        {
            int particlesPerPoint = totalParticles / EmitPositions.Count;
            int remainingParticles = totalParticles % EmitPositions.Count;

            foreach (var position in EmitPositions)
            {
                int particlesToEmit = particlesPerPoint + (remainingParticles > 0 ? 1 : 0);
                if (remainingParticles > 0) remainingParticles--;

                for (int i = 0; i < particlesToEmit; i++)
                {
                    emitParams.position = position;
                    particleSystem.Emit(emitParams, 1);
                }
            }
        }
        else
        {
            Transform cameraTransform = Camera.main.transform;
            float distanceFromCamera = .5f;
            Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distanceFromCamera);

            float emissionScale = .25f;

            for (int i = 0; i < totalParticles; i++)
            {
                emitParams.position = targetPosition + UnityEngine.Random.insideUnitSphere * emissionScale;
                particleSystem.Emit(emitParams, 1);
            }
        }
    }

    #endregion
    #region Initialize Native Arrays
    public void InitializeNativeArrays()
    {
        #region Noise
        nativeAttractionNoise = new NativeArray<float>(particleSystem.particleCount, Allocator.Persistent);
        nativePositionNoise = new NativeArray<float>(particleSystem.particleCount, Allocator.Persistent);
        nativeTheOtherFactors = new NativeArray<float>(particleSystem.particleCount, Allocator.Persistent);
        nativeIndexStepSize = new NativeArray<int>(particleSystem.particleCount, Allocator.Persistent);
        #endregion
        #region Joints
        nativeJointPositions = new NativeArray<Vector3>(Joints.Count, Allocator.Persistent);
        nativeJointRotations = new NativeArray<quaternion>(Joints.Count, Allocator.Persistent);
        #endregion
        #region Last Frame Positions and Velocities
        nativePreviousVelocities = new NativeArray<Vector3>(particleSystem.particleCount, Allocator.Persistent);
        nativePreviousPositions = new NativeArray<Vector3>(particleSystem.particleCount, Allocator.Persistent);
        #endregion
        #region Mesh Positions
        nativeRelativePositionsLeft = new NativeArray<Vector3>(100000, Allocator.Persistent);
        nativeRelativePositionsRight = new NativeArray<Vector3>(100000, Allocator.Persistent);
        nativeJointIndicesLeft = new NativeArray<int>(100000, Allocator.Persistent);
        nativeJointIndicesRight = new NativeArray<int>(100000, Allocator.Persistent);
        #endregion
    }
    #endregion
    #region  Assign Joint Indices And Relative Positions
    public void AssignJointIndicesAndRelativePositions()
    {
        for (int i = 0; i < outputLeftHandJointIndices.Count; i++)
        {
            nativeJointIndicesRight[i] = outputLeftHandJointIndices[i];
            nativeRelativePositionsRight[i] = outputLeftHandRelativePositions[i];
            maxIndexRightHand = outputLeftHandJointIndices.Count;

            nativeJointIndicesLeft[i] = outputLeftHandJointIndices[i];
            nativeRelativePositionsLeft[i] = outputLeftHandRelativePositions[i];
            maxIndexLeftHand = outputLeftHandJointIndices.Count;

        }
    }
    #endregion
    #region Start Engine
    public IEnumerator StartEngine()
    {
        InitializeJob();
        yield return null;
        EngineOn = true;
        UpdateGaussAndPositionNoise();
    }
    private void InitializeJob()
    {
        particleAttractionJob = new ParticleAttractionJob
        {
            paJob_VelocityNoise = nativeAttractionNoise,
            paJob_PositionNoise = nativePositionNoise,
            paJob_TheOtherFactors = nativeTheOtherFactors,
            paJob_IndexStepSizes = nativeIndexStepSize,
            paJob_JointPositions = nativeJointPositions,
            paJob_JointRotations = nativeJointRotations,
            paJob_SpeedLimit = SpeedLimitMax,
            paJob_relativePositionsLeft = nativeRelativePositionsLeft,
            paJob_relativePositionsRight = nativeRelativePositionsRight,
            paJob_jointIndicesLeft = nativeJointIndicesLeft,
            paJob_jointIndicesRight = nativeJointIndicesRight,
            paJob_leftMaxIndex = maxIndexLeftHand,
            paJob_rightMaxIndex = maxIndexRightHand,
            paJob_Gauss = .25f,
            paJob_previousVelocities = nativePreviousVelocities,
            paJob_previousPositions = nativePreviousPositions,
            paJob_CurrentTrailIndexOffset = currentTrailIndexOffset,
            paJob_Time = Vector3.one,
        };
    }
    private void UpdateGaussAndPositionNoise()
    {
        #region Oscillations
        #region Gauss
        GaussNoiseMaxMin = OscGaussNoiseMaxMin ? OscillateValue(GaussNoiseMaxMinOscSpeed,  GaussNoiseMaxMinMin, GaussNoiseMaxMinMax) : GaussNoiseMaxMin;
        GaussNoiseMin = OscGaussNoiseMin ? OscillateValue(GaussNoiseMinOscSpeed,  GaussNoiseMinMin, GaussNoiseMinMax) : GaussNoiseMin;
        GaussNoiseMax = OscGaussNoiseMax ? OscillateValue(GaussNoiseMaxOscSpeed,  GaussNoiseMaxMin, GaussNoiseMaxMax) : GaussNoiseMax;
        #endregion
        #region Position
        PosNoiseMin = OscPosNoiseMin ? OscillateValue(PosNoiseMinOscSpeed,  PosNoiseMinMin, GaussNoiseMinMax) : PosNoiseMin;
        #endregion
        #region Step Size
        IndexStepSizeMax = OscIndexStepSizeMax ? OscillateValue(IndexStepSizeMaxOscSpeed, IndexStepSizeMaxMin, IndexStepSizeMaxMax) : IndexStepSizeMax;
        #endregion
        #endregion
        float mean = (GaussNoiseMin + GaussNoiseMax) / 2f;
        float positionMean = (PosNoiseMin + PosNoiseMax) / 2f;
        float theOtherMean = (TheOtherFactorMin + TheOtherFactorMax) / 2f;
        float indexMean = (IndexStepSizeMin + IndexStepSizeMax) / 2f;

        float standardDeviation = Mathf.Abs(GaussNoiseMax - mean) / 3f; // Adjust as needed
        float positionSTD = Mathf.Abs(PosNoiseMax - positionMean) / 3f;
        float theOtherSTD = Mathf.Abs(TheOtherFactorMax - theOtherMean) / 3f;
        float indexSTD = Mathf.Abs(IndexStepSizeMax - indexMean) / 3f;

        for (int i = 0; i < nativeAttractionNoise.Length; i++)
        {
            particleAttractionJob.paJob_VelocityNoise[i] = GenerateRandomValueWithNormalDistribution(mean, standardDeviation, GaussNoiseMin, GaussNoiseMax);
            particleAttractionJob.paJob_PositionNoise[i] = GenerateRandomValueWithNormalDistribution(positionMean, positionSTD, PosNoiseMin, PosNoiseMax);
            if (i % 2 == 0)
            {
                particleAttractionJob.paJob_IndexStepSizes[i] = 1;
                particleAttractionJob.paJob_TheOtherFactors[i] = 0;
            }
            else
            {
                int randomIndexStepSize = GenerateRandomValueWithNormalDistribution(indexMean, indexSTD, IndexStepSizeMin, IndexStepSizeMax);
                particleAttractionJob.paJob_IndexStepSizes[i] = (particleAttractionJob.paJob_IndexStepSizes[i] + randomIndexStepSize) % nativeJointIndicesLeft.Length;
                particleAttractionJob.paJob_TheOtherFactors[i] = GenerateRandomValueWithNormalDistribution(theOtherMean, theOtherSTD, TheOtherFactorMin, TheOtherFactorMax);
            }
        }
    }
    #endregion
    #region Engine Update
    private int GenerateRandomValueWithNormalDistribution(float mean, float standardDeviation, float min, float max)
    {
        float floatVal = GenerateRandomValueWithNormalDistributionAsFloat(mean, standardDeviation);
        float clampedVal = Mathf.Clamp(floatVal, min, max);
        return Mathf.RoundToInt(clampedVal);
    }
    private float GenerateRandomValueWithNormalDistributionAsFloat(float mean, float standardDeviation)
    {
        float u1 = 1f - UnityEngine.Random.value;
        float u2 = 1f - UnityEngine.Random.value;
        float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
        return mean + standardDeviation * randStdNormal;
    }
    private static int OscillateValue(float speed, int minVal, int maxVal)
    {
        float floatVal = OscillateValue(speed, (float)minVal, (float)maxVal);
        return Mathf.RoundToInt(floatVal);
    }
    private static float OscillateValue(float speed, float minVal, float maxVal)
    {
        float amplitude = (maxVal - minVal) / 2.0f;
        float midpoint = (maxVal + minVal) / 2.0f;
        float oscillatedValue = midpoint + amplitude * Mathf.Sin(speed * Time.time);
        return oscillatedValue;
    }
    private void UpdateJointPositions()
    {
        for (int i = 0; i < Joints.Count; i++)
        {
            nativeJointPositions[i] = Joints[i].position;
            nativeJointRotations[i] = Joints[i].rotation;
        }
    }
    private void UpdateJobVariables()
    {
        #region Randomize Oscillation Speeds
        float mean = (OscSpeedMin + OscSpeedMax) / 2f;
        float standardDeviation = Mathf.Abs(OscSpeedMax - mean) / 3f; // Adjust as needed

        GaussOscSpeed = GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax);
        GaussNoiseMinOscSpeed = GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax);
        GaussNoiseMaxOscSpeed = GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax);
        GaussNoiseMaxMinOscSpeed = GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax);
        PosNoiseMinOscSpeed = GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax);
        IndexStepSizeMaxOscSpeed = GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax);
        #endregion

        #region Joints
        particleAttractionJob.paJob_JointPositions = nativeJointPositions;
        particleAttractionJob.paJob_JointRotations = nativeJointRotations;
        #endregion
        #region Speed Limit
        SpeedLimit = OscSpeedLimit ? OscillateValue(SpeedLimitOscSpeed,  SpeedLimitMin, SpeedLimitMax) : SpeedLimit;
        particleAttractionJob.paJob_SpeedLimit = SpeedLimit;
        #endregion
        #region Index Offset
        currentTrailIndexOffset = currentTrailIndexOffset % nativeJointIndicesLeft.Length; // keep an eye on this, only works if same length for both ahnds
        currentTrailIndexOffset += 1;
        particleAttractionJob.paJob_CurrentTrailIndexOffset = currentTrailIndexOffset;
        #endregion
        #region Gauss
        Gauss = OscGauss ? OscillateValue(GaussOscSpeed,  GaussMin, GaussMax) : Gauss;
        particleAttractionJob.paJob_Gauss = Gauss;
        #endregion
        #region Mesh Positions
        particleAttractionJob.paJob_relativePositionsLeft = nativeRelativePositionsLeft;
        particleAttractionJob.paJob_relativePositionsRight = nativeRelativePositionsLeft;
        particleAttractionJob.paJob_jointIndicesLeft = nativeJointIndicesLeft;
        particleAttractionJob.paJob_jointIndicesRight = nativeJointIndicesLeft;
        #endregion
        #region Hands
        particleAttractionJob.paJob_leftMaxIndex = maxIndexLeftHand / 2;
        particleAttractionJob.paJob_rightMaxIndex = maxIndexRightHand / 2;
        #endregion
        #region Color   
        #region Min Oscillation
        MinRGB.r = OscColorMin ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MinRGBMin.r, MinRGBMax.r) : MinRGB.r;
        MinRGB.g = OscColorMin ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MinRGBMin.g, MinRGBMax.g) : MinRGB.g;
        MinRGB.b = OscColorMin ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MinRGBMin.b, MinRGBMax.b) : MinRGB.b;
        #endregion
        #region Max Oscillation
        MaxRGB.r = OscColorMax ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MaxRGBMin.r, MaxRGBMax.r) : MaxRGB.r;
        MaxRGB.g = OscColorMax ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MaxRGBMin.g, MaxRGBMax.g) : MaxRGB.g;
        MaxRGB.b = OscColorMax ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MaxRGBMin.b, MaxRGBMax.b) : MaxRGB.b;
        #endregion

        RGB.r = OscColor ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MinRGB.r, MaxRGB.r) : RGB.r;
        RGB.g = OscColor ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MinRGB.g, MaxRGB.g) : RGB.g;
        RGB.b = OscColor ? OscillateValue(GenerateRandomValueWithNormalDistribution(mean, standardDeviation, OscSpeedMin, OscSpeedMax),  MinRGB.b, MaxRGB.b) : RGB.b;

        particleAttractionJob.paJob_Time.x = RGB.r;
        particleAttractionJob.paJob_Time.y = RGB.g;
        particleAttractionJob.paJob_Time.z = RGB.b;
        #endregion
    }
    #endregion
    #region Engine Job   
    void OnParticleUpdateJobScheduled()
    {
        if (EngineOn && particleAttractionJobHandle.IsCompleted)
        {
            particleAttractionJobHandle.Complete();
            UpdateGaussAndPositionNoise();
            UpdateJointPositions();
            UpdateJobVariables();
            particleAttractionJobHandle = particleAttractionJob.ScheduleBatch(particleSystem, 1024);
        }
    }

    [BurstCompile]
    struct ParticleAttractionJob : IJobParticleSystemParallelForBatch
    {
        #region Job Variables
        [ReadOnly] public NativeArray<float> paJob_VelocityNoise;
        [ReadOnly] public NativeArray<float> paJob_PositionNoise;
        [ReadOnly] public NativeArray<Vector3> paJob_JointPositions;
        [ReadOnly] public NativeArray<quaternion> paJob_JointRotations;
        [ReadOnly] public float paJob_SpeedLimit;

        [ReadOnly] public NativeArray<Vector3> paJob_relativePositionsLeft;
        [ReadOnly] public NativeArray<Vector3> paJob_relativePositionsRight;
        [ReadOnly] public NativeArray<int> paJob_jointIndicesLeft;
        [ReadOnly] public NativeArray<int> paJob_jointIndicesRight;
        [ReadOnly] public int paJob_leftMaxIndex;
        [ReadOnly] public int paJob_rightMaxIndex;

        [ReadOnly] public Vector3 paJob_Time;
        [ReadOnly] public int paJob_CurrentTrailIndexOffset;

        [ReadOnly] public float paJob_Gauss;

        [ReadOnly] public NativeArray<float> paJob_TheOtherFactors;
        [ReadOnly] public NativeArray<int> paJob_IndexStepSizes;

        public NativeArray<Vector3> paJob_previousVelocities;
        public NativeArray<Vector3> paJob_previousPositions;

        #endregion
        public void Execute(ParticleSystemJobData particles, int startIndex, int count)
        {
            #region Basic Setup
            var positions = particles.positions;
            var velocities = particles.velocities;
            var colors = particles.startColors;
            int endIndex = startIndex + count;
            int halfCount = particles.count / 2;
            #endregion

            for (int i = startIndex; i < math.min(endIndex, halfCount); i++)
            {
                #region Variable Setup
                int particleIndex = i;
                //int offsetIndex = (i + paJob_CurrentTrailIndexOffset) % paJob_leftMaxIndex;
                int offsetIndex = (i + paJob_IndexStepSizes[particleIndex]) % paJob_leftMaxIndex;
                Vector3 particlePosition = positions[particleIndex];
                Vector3 previousVelocity = paJob_previousVelocities[particleIndex];
                int jointIndex = paJob_jointIndicesLeft[offsetIndex];
                var jointPosition = paJob_JointPositions[jointIndex];
                var jointRotation = paJob_JointRotations[jointIndex];
                var relativePosition = paJob_relativePositionsLeft[offsetIndex];
                #endregion
                #region Compute World Position
                Vector3 worldPosition = ComputeWorldPosition(jointPosition, jointRotation, relativePosition, paJob_PositionNoise[particleIndex]);
                #endregion
                #region Calculate Attraction Velocity
                Vector3 velocity1 = CalculateAttractionVelocity(worldPosition, particlePosition, velocities[particleIndex], paJob_Gauss, 0.000001f);
                #endregion
                #region Apply Velocity Noise
                velocity1 *= paJob_VelocityNoise[particleIndex];
                #endregion

                #region 2cnd Hand
                offsetIndex = halfCount + ((i + paJob_IndexStepSizes[particleIndex]) % paJob_rightMaxIndex);
                particlePosition = positions[particleIndex];
                previousVelocity = paJob_previousVelocities[particleIndex];
                jointIndex = paJob_jointIndicesLeft[offsetIndex];
                jointPosition = paJob_JointPositions[jointIndex];
                jointRotation = paJob_JointRotations[jointIndex];
                relativePosition = paJob_relativePositionsLeft[offsetIndex];
                #endregion
                #region Compute World Position
                worldPosition = ComputeWorldPosition(jointPosition, jointRotation, relativePosition, paJob_PositionNoise[particleIndex]);
                #endregion
                #region Calculate Attraction Velocity
                Vector3 velocity2 = CalculateAttractionVelocity(worldPosition, particlePosition, velocities[particleIndex], paJob_Gauss, 0.000001f);
                #endregion
                #region Apply Velocity Noise
                velocity2 *= paJob_VelocityNoise[particleIndex];
                #endregion
                Vector3 velocity = (velocity1 + (velocity2 * paJob_TheOtherFactors[particleIndex])) / 2;// math.lerp(velocity1, velocity2, paJob_TheOtherFactors[particleIndex]);
                int factor = paJob_TheOtherFactors[particleIndex] == 0 ? 2 : 1;
                velocity *= factor;

                #region Apply Speed Adjustments
                float trailSpeed = math.length(velocity);
                float distance = math.length(worldPosition - particlePosition);
                float localSpeedLimit = distance > 1 ? paJob_SpeedLimit : paJob_SpeedLimit * distance;
                localSpeedLimit = localSpeedLimit < 10 ? 10f : localSpeedLimit;
                //localSpeedLimit += paJob_SpeedLimit + (10 * (particleIndex % 2));
                velocity = ApplySpeedAdjustments(paJob_previousPositions[particleIndex], worldPosition, velocity, paJob_VelocityNoise[particleIndex], localSpeedLimit * distance, trailSpeed);
                #endregion
                #region Update Position
                float deltaTime = .00000001f;
                particlePosition += 0.5f * (previousVelocity + velocity) * deltaTime;
                #endregion
                #region Compute Color
                Color finalColor = ComputeParticleColor(velocity, particlePosition, worldPosition, trailSpeed, paJob_Time);
                #endregion
                #region Update Arrays
                velocities[particleIndex] = math.lerp(velocities[particleIndex], velocity, .1f);
                positions[particleIndex] = particlePosition;
                paJob_previousVelocities[particleIndex] = velocity;
                paJob_previousPositions[particleIndex] = worldPosition;
                colors[particleIndex] = Color.Lerp(colors[particleIndex], finalColor, .05f);
                #endregion
            }
            for (int i = math.max(startIndex, halfCount); i < endIndex; i++)
            {
                #region Variable Setup
                int particleIndex = i;
                //int offsetIndex = halfCount + ((i + paJob_CurrentTrailIndexOffset) % paJob_rightMaxIndex);
                int offsetIndex = halfCount + ((i + paJob_IndexStepSizes[particleIndex]) % paJob_rightMaxIndex);
                Vector3 particlePosition = positions[particleIndex];
                Vector3 previousVelocity = paJob_previousVelocities[particleIndex];
                int jointIndex = paJob_jointIndicesLeft[offsetIndex];
                var jointPosition = paJob_JointPositions[jointIndex];
                var jointRotation = paJob_JointRotations[jointIndex];
                var relativePosition = paJob_relativePositionsLeft[offsetIndex];
                #endregion
                #region Compute World Position
                Vector3 worldPosition = ComputeWorldPosition(jointPosition, jointRotation, relativePosition, paJob_PositionNoise[particleIndex]);
                #endregion
                #region Calculate Attraction Velocity
                Vector3 velocity1 = CalculateAttractionVelocity(worldPosition, particlePosition, velocities[particleIndex], paJob_Gauss, 0.000001f);
                #endregion
                #region Apply Velocity Noise
                velocity1 *= paJob_VelocityNoise[particleIndex];
                #endregion

                #region 2cnd Hand
                offsetIndex = (i + paJob_IndexStepSizes[particleIndex]) % paJob_leftMaxIndex;
                particlePosition = positions[particleIndex];
                previousVelocity = paJob_previousVelocities[particleIndex];
                jointIndex = paJob_jointIndicesLeft[offsetIndex];
                jointPosition = paJob_JointPositions[jointIndex];
                jointRotation = paJob_JointRotations[jointIndex];
                relativePosition = paJob_relativePositionsLeft[offsetIndex];
                #endregion
                #region Compute World Position
                worldPosition = ComputeWorldPosition(jointPosition, jointRotation, relativePosition, paJob_PositionNoise[particleIndex]);
                #endregion
                #region Calculate Attraction Velocity
                Vector3 velocity2 = CalculateAttractionVelocity(worldPosition, particlePosition, velocities[particleIndex], paJob_Gauss, 0.000001f);
                #endregion
                #region Apply Velocity Noise
                velocity2 *= paJob_VelocityNoise[particleIndex];
                #endregion

                Vector3 velocity = (velocity1 + (velocity2 * paJob_TheOtherFactors[particleIndex])) / 2;// math.lerp(velocity1, velocity2, paJob_TheOtherFactors[particleIndex]);
                int factor = paJob_TheOtherFactors[particleIndex] == 0 ? 2 : 1;
                velocity *= factor;

                #region Apply Speed Adjustments
                float trailSpeed = math.length(velocity);
                float distance = math.length(worldPosition - particlePosition);
                float localSpeedLimit = distance > 1 ? paJob_SpeedLimit : paJob_SpeedLimit * distance;
                localSpeedLimit = localSpeedLimit < 10 ? 10f : localSpeedLimit;
                //localSpeedLimit += paJob_SpeedLimit + (10 * (particleIndex % 2));
                velocity = ApplySpeedAdjustments(paJob_previousPositions[particleIndex], worldPosition, velocity, paJob_VelocityNoise[particleIndex], localSpeedLimit * distance, trailSpeed);
                #endregion
                #region Update Position
                float deltaTime = .00000001f;
                particlePosition += 0.5f * (previousVelocity + velocity) * deltaTime;
                #endregion
                #region Compute Color
                Color finalColor = ComputeParticleColor(velocity, particlePosition, worldPosition, trailSpeed, paJob_Time);
                #endregion
                #region Update Arrays
                velocities[particleIndex] = math.lerp(velocities[particleIndex], velocity, .1f);
                positions[particleIndex] = particlePosition;
                paJob_previousVelocities[particleIndex] = velocity;
                paJob_previousPositions[particleIndex] = worldPosition;
                colors[particleIndex] = Color.Lerp(colors[particleIndex], finalColor, .05f);
                #endregion
            }
        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Vector3 ComputeWorldPosition(Vector3 jointPosition, Quaternion jointRotation, Vector3 relativePosition, float positionNoise)
        {
            // Calculate the world position based on joint data and relative position
            Vector3 worldPosition = jointPosition + (Vector3)math.mul(jointRotation, relativePosition);

            // Apply noise to the world position
            worldPosition = Vector3.Lerp(worldPosition, jointPosition, positionNoise);

            return worldPosition;
        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Vector3 ApplySpeedAdjustments(Vector3 previousPosition, Vector3 worldPosition, Vector3 velocity, float velocityNoise, float speedLimit, float trailSpeed)
        {
            /*
            // Calculate vertex distance with a cap
            float vertexDistance = math.length(previousPosition - worldPosition);
            vertexDistance = vertexDistance > .1f ? .1f : vertexDistance;

            // Calculate the speed factor
            float speedFactor = vertexDistance * 10000f;

            // Apply noise and adjust velocity1
            velocity *= (speedFactor / 800) + velocityNoise;
            */
            velocity = math.select(velocity, (speedLimit / trailSpeed) * velocity, trailSpeed > speedLimit);

            return velocity;
        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Vector3 CalculateAttractionVelocity(Vector3 worldPosition, Vector3 particlePosition, Vector3 currentVelocity, float attractionStrength, float epsilon)
        {
            // Calculate the direction and distance from the particle to the world position
            Vector3 direction = worldPosition - particlePosition;
            float distance = math.length(direction);

            // Adjust the attraction strength by adding a small epsilon to avoid division by zero
            float adjustedAttractionStrength = attractionStrength + epsilon;

            // Normalize the direction vector
            Vector3 normDirection = math.normalize(direction);

            // Calculate the exponent for the Gaussian distribution based on the distance and attraction strength
            float exponent = -(math.lengthsq(direction)) / (2 * adjustedAttractionStrength * adjustedAttractionStrength);

            // Apply the Gaussian distribution to calculate the attraction force
            float attraction = (1 / (math.pow(2 * math.PI, 1.5f) * adjustedAttractionStrength)) * math.exp(exponent);

            // Apply a minimum attraction force to ensure particles do not come to a complete stop
            float minAttraction = .01f;
            attraction = math.select(attraction * (1 - (distance * distance)), minAttraction, distance >= 1);

            // Calculate and return the new velocity1 of the particle
            return currentVelocity + (normDirection * attraction);
        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static Color ComputeParticleColor(Vector3 velocity, Vector3 particlePosition, Vector3 worldPosition, float trailSpeed, float3 timeLimits)
        {
            // Normalize the velocity1 and calculate the unclamped velocity1 color
            Vector3 normVelocity = math.normalize(velocity);
            Color unclampedVelocityColor = new Color(math.abs(normVelocity.x), math.abs(normVelocity.y), math.abs(normVelocity.z), 1);

            // Calculate the default color based on the trail speed and distance
            float offsetColor = .2f + (trailSpeed * math.length(particlePosition - worldPosition));
            Color defaultColor = Color.blue * .42f;// new Color(offsetColor, offsetColor, offsetColor, 1);

            // Clamp the velocity1 color to the specified time limits
            float clampedR = math.min(unclampedVelocityColor.r, timeLimits.x);
            float clampedG = math.min(unclampedVelocityColor.g, timeLimits.y);
            float clampedB = math.min(unclampedVelocityColor.b, timeLimits.z);
            Color velocityColor = new Color(clampedR, clampedG, clampedB, 1);

            // Interpolate between the velocity1 color and the default color
            Color finalColor = Color.Lerp(velocityColor, defaultColor, .55f);

            return finalColor;
        }
    }
    void OnDisable()
    {
        particleAttractionJobHandle.Complete();
        nativeAttractionNoise.Dispose();
        nativePositionNoise.Dispose();
        nativeTheOtherFactors.Dispose();
        nativeJointPositions.Dispose();
        nativeJointRotations.Dispose();
        nativePreviousVelocities.Dispose();
        nativeJointIndicesLeft.Dispose();
        nativeJointIndicesRight.Dispose();
        nativeRelativePositionsLeft.Dispose();
        nativeRelativePositionsRight.Dispose();
        nativeIndexStepSize.Dispose();
    }
    #endregion
}