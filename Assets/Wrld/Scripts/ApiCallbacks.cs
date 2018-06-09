using System;
using Wrld.Concurrency;
using Wrld.MapCamera;
using Wrld.Materials;
using Wrld.Meshes;
using Wrld.Precaching;
using Wrld.Resources.Buildings;
using Wrld.Resources.IndoorMaps;
using Wrld.Resources.Labels;
using Wrld.Space.Positioners;

// Disable "Private field is assigned but never used" warning spam, as these are used by native library interop.
#pragma warning disable 0414

namespace Wrld
{
    internal struct LabelCallbacks
    {
        private LabelCallbacks(
            IntPtr _labelServiceHandle,
            LabelServiceInternal.AddLabelDelegate _addLabel,
            LabelServiceInternal.UpdateLabelDelegate _updateLabel,
            LabelServiceInternal.RemoveLabelDelegate _removeLabel)
        {
            labelServiceHandle = _labelServiceHandle;
            addLabel = _addLabel;
            updateLabel = _updateLabel;
            removeLabel = _removeLabel;
        }

        public static LabelCallbacks Create(IntPtr labelServiceHandle)
        {
            return new LabelCallbacks(
                labelServiceHandle,
                LabelServiceInternal.AddLabel,
                LabelServiceInternal.UpdateLabel,
                LabelServiceInternal.RemoveLabel);
        }

        IntPtr labelServiceHandle;
        LabelServiceInternal.AddLabelDelegate addLabel;
        LabelServiceInternal.UpdateLabelDelegate updateLabel;
        LabelServiceInternal.RemoveLabelDelegate removeLabel;
    }

    internal struct TextureCallbacks
    {
        private TextureCallbacks(
            IntPtr _textureServiceHandle,
            TextureLoadHandler.AllocateTextureBufferCallback _allocateTextureBuffer,
            TextureLoadHandler.BeginUploadTextureBufferCallback _beginUploadTextureBuffer,
            IndoorMapTextureStreamingService.OnTextureReceivedCallback _onTextureReceived,
            TextureLoadHandler.ReleaseTextureCallback _releaseTexture,
            TextureLoadHandler.CreateCubemapFromFacesCallback _createCubemapFromFaces)
        {
            textureServiceHandle = _textureServiceHandle;
            allocateTextureBuffer = _allocateTextureBuffer;
            beginUploadTextureBuffer = _beginUploadTextureBuffer;
            onTextureReceived = _onTextureReceived;
            releaseTexture = _releaseTexture;
            createCubemapFromFaces = _createCubemapFromFaces;
        }

        public static TextureCallbacks Create(IntPtr textureServiceHandle)
        {
            return new TextureCallbacks(
                textureServiceHandle,
                TextureLoadHandler.AllocateTextureBuffer,
                TextureLoadHandler.BeginUploadTextureBuffer,
                IndoorMapTextureStreamingService.OnTextureReceived,
                TextureLoadHandler.ReleaseTexture,
                TextureLoadHandler.CreateCubemapFromFaces);
        }

        IntPtr textureServiceHandle;
        TextureLoadHandler.AllocateTextureBufferCallback allocateTextureBuffer;
        TextureLoadHandler.BeginUploadTextureBufferCallback beginUploadTextureBuffer;
        IndoorMapTextureStreamingService.OnTextureReceivedCallback onTextureReceived;
        TextureLoadHandler.ReleaseTextureCallback releaseTexture;
        TextureLoadHandler.CreateCubemapFromFacesCallback createCubemapFromFaces;
    }

    internal struct ThreadServiceCallbacks
    {
        private ThreadServiceCallbacks(
            IntPtr _threadServiceHandle,
            ThreadService.CreateThreadDelegate _createThread,
            ThreadService.JoinThreadDelegate _joinThread)
        {
            threadServiceHandle = _threadServiceHandle;
            createThread = _createThread;
            joinThread = _joinThread;
        }

        public static ThreadServiceCallbacks Create(IntPtr threadServiceHandle)
        {
            return new ThreadServiceCallbacks(
                threadServiceHandle,
                ThreadService.CreateThread,
                ThreadService.JoinThread);
        }

        IntPtr threadServiceHandle;
        ThreadService.CreateThreadDelegate createThread;
        ThreadService.JoinThreadDelegate joinThread;
    }

    internal struct SceneCallbacks
    {
        private SceneCallbacks(
            IntPtr _sceneServiceCallbacks,
            MapGameObjectScene.SetScaleCallback _setScale,
            MapGameObjectScene.SetTranslationCallback _setTranslation,
            MapGameObjectScene.SetOrientationCallback _setRotation,
            MapGameObjectScene.SetColorCallback _setColor)
        {
            sceneServiceCallbacks = _sceneServiceCallbacks;
            setScale = _setScale;
            setTranslation = _setTranslation;
            setRotation = _setRotation;
            setColor = _setColor;
        }

        public static SceneCallbacks Create(IntPtr sceneServiceCallbacks)
        {
            return new SceneCallbacks(
                sceneServiceCallbacks,
                MapGameObjectScene.SetScale,
                MapGameObjectScene.SetTranslation,
                MapGameObjectScene.SetOrientation,
                MapGameObjectScene.SetColor);
        }

        IntPtr sceneServiceCallbacks;
        MapGameObjectScene.SetScaleCallback setScale;
        MapGameObjectScene.SetTranslationCallback setTranslation;
        MapGameObjectScene.SetOrientationCallback setRotation;
        MapGameObjectScene.SetColorCallback setColor;
    }

    internal struct MeshCallbacks
    {
        private MeshCallbacks(
            IntPtr _sceneServiceHandle,
            MeshUploader.AllocateUnpackedMeshCallback _allocateUnpackedMesh,
            MeshUploader.UploadUnpackedMeshCallback _uploadUnpackedMesh,
            MapGameObjectScene.AddMeshCallback _addMesh,
            MapGameObjectScene.DeleteMeshCallback _deleteMesh,
            MapGameObjectScene.VisibilityCallback _setVisible)
        {
            sceneServiceHandle = _sceneServiceHandle;
            allocateUnpackedMesh = _allocateUnpackedMesh;
            uploadUnpackedMesh = _uploadUnpackedMesh;
            addMesh = _addMesh;
            deleteMesh = _deleteMesh;
            setVisible = _setVisible;
        }

        public static MeshCallbacks Create(IntPtr sceneServiceHandle)
        {
            return new MeshCallbacks(
                sceneServiceHandle,
                MeshUploader.AllocateUnpackedMesh,
                MeshUploader.UploadUnpackedMesh,
                MapGameObjectScene.AddMesh,
                MapGameObjectScene.DeleteMesh,
                MapGameObjectScene.SetVisible);
        }

        IntPtr sceneServiceHandle;
        MeshUploader.AllocateUnpackedMeshCallback allocateUnpackedMesh;
        MeshUploader.UploadUnpackedMeshCallback uploadUnpackedMesh;
        MapGameObjectScene.AddMeshCallback addMesh;
        MapGameObjectScene.DeleteMeshCallback deleteMesh;
        MapGameObjectScene.VisibilityCallback setVisible;
    }

    internal struct IndoorMapCallbacks
    {
        private IndoorMapCallbacks(
            IntPtr _indoorMapsApiHandle,
            IntPtr _indoorMapsMaterialServiceHandle,
            IntPtr _indoorMapsSceneHandle,
            IndoorMapScene.AddRenderableCallback _addRenderable,
            IndoorMapScene.AddRenderableCallback _addHighlightRenderable,
            IndoorMapScene.AddInstancedRenderableCallback _addInstancedRenderable,
            IndoorMapScene.OnRenderStateUpdatedCallback _onRenderableStateUpdated,
            IndoorMapScene.OnRenderStateUpdatedCallback _onRenderableStateUpdatedForHighlightRenderable,
            IndoorMapScene.OnRenderStateUpdatedCallback _onInstancedRenderableStateUpdated,
            IndoorMapScene.SetMaterialCallback _setMaterial,
            IndoorMapScene.SetMaterialCallback _setMaterialForInstancedRenderable,
            IndoorMapsApiInternal.IndoorMapEventHandler _onIndoorMapEnteredCallback,
            IndoorMapsApiInternal.IndoorMapEventHandler _onIndoorMapExitedCallback,
            IndoorMapsApiInternal.IndoorMapEventHandler _onIndoorMapFloorChangedCallback,            
            IndoorMapsApiInternal.IndoorMapEntitiesClickHandler _onIndoorMapEntitiesClickedCallback,
            IndoorMapMaterialService.CreateMaterialCallback _createMaterial,
            IndoorMapMaterialService.DeleteMaterialCallback _deleteMaterial)
        {
            indoorMapsApiHandle = _indoorMapsApiHandle;
            indoorMapsMaterialServiceHandle = _indoorMapsMaterialServiceHandle;
            indoorMapsSceneHandle = _indoorMapsSceneHandle;
            addRenderable = _addRenderable;
            addHighlightRenderable = _addHighlightRenderable;
            addInstancedRenderable = _addInstancedRenderable;
            onRenderableStateUpdated = _onRenderableStateUpdated;
            onRenderableStateUpdatedForHighlightRenderable = _onRenderableStateUpdatedForHighlightRenderable;
            onInstancedRenderableStateUpdated = _onInstancedRenderableStateUpdated;
            setMaterial = _setMaterial;
            setMaterialForInstancedRenderable = _setMaterialForInstancedRenderable;
            onIndoorMapEnteredCallback = _onIndoorMapEnteredCallback;
            onIndoorMapExitedCallback = _onIndoorMapExitedCallback;
            onIndoorMapFloorChangedCallback = _onIndoorMapFloorChangedCallback;            
            onIndoorMapEntitiesClickedCallback = _onIndoorMapEntitiesClickedCallback;
            createMaterial = _createMaterial;
            deleteMaterial = _deleteMaterial;
        }

        public static IndoorMapCallbacks Create(IntPtr _indoorMapsApiHandle, IntPtr _indoorMapsMaterialServiceHandle, IntPtr _indoorMapsSceneHandle)
        {
            return new IndoorMapCallbacks(
                _indoorMapsApiHandle,
                _indoorMapsMaterialServiceHandle,
                _indoorMapsSceneHandle,
                IndoorMapScene.AddRenderable,
                IndoorMapScene.AddHighlightRenderable,
                IndoorMapScene.AddInstancedRenderable,
                IndoorMapScene.OnRenderStateUpdated,
                IndoorMapScene.OnRenderStateUpdatedForHighlightRenderable,
                IndoorMapScene.OnRenderStateUpdatedForInstancedRenderable,
                IndoorMapScene.SetMaterial,
                IndoorMapScene.SetMaterialForInstancedRenderable,
                IndoorMapsApiInternal.OnIndoorMapEnteredCallback,
                IndoorMapsApiInternal.OnIndoorMapExitedCallback,
                IndoorMapsApiInternal.OnIndoorMapFloorChangedCallback,                
                IndoorMapsApiInternal.OnIndoorMapEntitiesClickedCallback,
                IndoorMapMaterialService.CreateMaterial,
                IndoorMapMaterialService.DeleteMaterial);
        }

        IntPtr indoorMapsApiHandle;
        IntPtr indoorMapsMaterialServiceHandle;
        IntPtr indoorMapsSceneHandle;
        IndoorMapScene.AddRenderableCallback addRenderable;
        IndoorMapScene.AddRenderableCallback addHighlightRenderable;
        IndoorMapScene.AddInstancedRenderableCallback addInstancedRenderable;
        IndoorMapScene.OnRenderStateUpdatedCallback onRenderableStateUpdated;
        IndoorMapScene.OnRenderStateUpdatedCallback onRenderableStateUpdatedForHighlightRenderable;
        IndoorMapScene.OnRenderStateUpdatedCallback onInstancedRenderableStateUpdated;
        IndoorMapScene.SetMaterialCallback setMaterial;
        IndoorMapScene.SetMaterialCallback setMaterialForInstancedRenderable;        
        IndoorMapsApiInternal.IndoorMapEventHandler onIndoorMapEnteredCallback;
        IndoorMapsApiInternal.IndoorMapEventHandler onIndoorMapExitedCallback;
        IndoorMapsApiInternal.IndoorMapEventHandler onIndoorMapFloorChangedCallback;        
        IndoorMapsApiInternal.IndoorMapEntitiesClickHandler onIndoorMapEntitiesClickedCallback;
        IndoorMapMaterialService.CreateMaterialCallback createMaterial;
        IndoorMapMaterialService.DeleteMaterialCallback deleteMaterial;
    }

    internal struct PositionerCallbacks
    {
        private PositionerCallbacks(
            IntPtr _positionerApiHandle,
            PositionerApiInternal.PositionerProjectionChangedDelegate _positionerChanged)
        {
            positionerApiHandle = _positionerApiHandle;
            positionerChanged = _positionerChanged;
        }

        public static PositionerCallbacks Create(IntPtr _positionerApiHandle)
        {
            return new PositionerCallbacks(
                _positionerApiHandle,
                PositionerApiInternal.OnPositionerUpdated);
        }

        IntPtr positionerApiHandle;
        PositionerApiInternal.PositionerProjectionChangedDelegate positionerChanged;
    }

    internal struct CameraCallbacks
    {
        private CameraCallbacks(
            IntPtr _cameraApiInternalHandle,
            CameraApiInternal.CameraEventCallback _cameraEventCallback)
        {
            cameraApiInternalHandle = _cameraApiInternalHandle;
            cameraEventCallback = _cameraEventCallback;
        }

        public static CameraCallbacks Create(IntPtr _cameraApiInternalHandle)
        {
            return new CameraCallbacks(
                _cameraApiInternalHandle,
                CameraApiInternal.OnCameraEvent);
        }

        IntPtr cameraApiInternalHandle;
        CameraApiInternal.CameraEventCallback cameraEventCallback;
    }

    internal struct BuildingHighlightCallbacks
    {
        private BuildingHighlightCallbacks(
            IntPtr _buildingsApiHandle,
            BuildingsApiInternal.BuildingHighlightChangedDelegate _buildingHighlightChanged)
        {
            buildingsApiHandle = _buildingsApiHandle;
            buildingHighlightChanged = _buildingHighlightChanged;
        }

        public static BuildingHighlightCallbacks Create(IntPtr buildingsApiHandle)
        {
            return new BuildingHighlightCallbacks(
                buildingsApiHandle,
                BuildingsApiInternal.OnBuildingHighlightChanged);
        }

        IntPtr buildingsApiHandle;
        BuildingsApiInternal.BuildingHighlightChangedDelegate buildingHighlightChanged;
    }

    internal struct PrecacheCallbacks
    {
        private PrecacheCallbacks(
            IntPtr _precacheApiHandle,
            PrecacheApiInternal.PrecacheOperationCompletedHandler _precacheOperationCompleted
            )
        {
            precacheApiHandle = _precacheApiHandle;
            precacheOperationCompleted = _precacheOperationCompleted;
        }

        public static PrecacheCallbacks Create(IntPtr buildingsApiHandle)
        {
            return new PrecacheCallbacks(
                buildingsApiHandle,
                PrecacheApiInternal.OnPrecacheOperationCompletedCallback);
        }

        IntPtr precacheApiHandle;
        PrecacheApiInternal.PrecacheOperationCompletedHandler precacheOperationCompleted;
    }

    internal struct ApiCallbacks
    {
        public ApiCallbacks(
            IntPtr indoorMapsApiHandle, 
            IntPtr indoorMapsMaterialServiceHandle, 
            IntPtr indoorMapsSceneHandle, 
            IntPtr cameraApiHandle,
            IntPtr buildingsApiHandle,
            IntPtr threadServiceHandle, 
            IntPtr textureServiceHandle, 
            IntPtr sceneServiceHandle, 
            IntPtr labelServiceHandle, 
            IntPtr positionerApiHandle,
            IntPtr precacheApiHandle)
        {
            meshCallbacks = MeshCallbacks.Create(sceneServiceHandle);
            cameraCallbacks = CameraCallbacks.Create(cameraApiHandle);
            assertHandlerCallback = AssertHandler.HandleAssert;
            fatalErrorHandlerCallback = FatalErrorHandler.HandleFatalError;
            textureCallbacks = TextureCallbacks.Create(textureServiceHandle);
            threadServiceCallbacks = ThreadServiceCallbacks.Create(threadServiceHandle);
            buildingHighlightCallbacks = BuildingHighlightCallbacks.Create(buildingsApiHandle);
            sceneCallbacks = SceneCallbacks.Create(sceneServiceHandle);
            indoorMapCallbacks = IndoorMapCallbacks.Create(indoorMapsApiHandle, indoorMapsMaterialServiceHandle, indoorMapsSceneHandle);
            positionerCallbacks = PositionerCallbacks.Create(positionerApiHandle);
            labelCallbacks = LabelCallbacks.Create(labelServiceHandle);
            precacheCallbacks = PrecacheCallbacks.Create(precacheApiHandle);
        }

        MeshCallbacks meshCallbacks;
        CameraCallbacks cameraCallbacks;
        AssertHandler.HandleAssertCallback assertHandlerCallback;
        FatalErrorHandler.HandleFatalErrorCallback fatalErrorHandlerCallback;
        TextureCallbacks textureCallbacks;
        ThreadServiceCallbacks threadServiceCallbacks;
        BuildingHighlightCallbacks buildingHighlightCallbacks;
        SceneCallbacks sceneCallbacks;
        IndoorMapCallbacks indoorMapCallbacks;
        PositionerCallbacks positionerCallbacks;
        LabelCallbacks labelCallbacks;
        PrecacheCallbacks precacheCallbacks;
    }
}

#pragma warning restore 0414