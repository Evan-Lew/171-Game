namespace NatureRendererDemo
{
    using UnityEngine;
#if UNITY_EDITOR
    using VisualDesignCafe.ShaderX.Editor.Drops;
#endif

    [ExecuteInEditMode]
    public class RenderPipelineLighting : MonoBehaviour
    {
        [SerializeField]
        private GameObject _standardLighting;

        [SerializeField]
        private Material _standardSky;

        [SerializeField]
        private Material _standardTerrain;

        [SerializeField]
        private GameObject _standardVolume;

        [SerializeField]
        private GameObject _universalLighting;

        [SerializeField]
        private Material _universalSky;

        [SerializeField]
        private Material _universalTerrain;

        [SerializeField]
        private GameObject _universalVolume;

        [SerializeField]
        private GameObject _highDefinitionLighting;

        [SerializeField]
        private Material _highDefinitionSky;

        [SerializeField]
        private GameObject _highDefinitionVolume;

        [SerializeField]
        private Material _highDefinitionTerrain;

        private void OnValidate()
        {
            Awake();
        }

        private void Awake()
        {
#if UNITY_EDITOR
            var renderPipeline = new RenderPipeline();

            if( _standardVolume != null )
                _standardVolume.SetActive( renderPipeline.Type == RenderPipelineType.Standard );

            if( _universalVolume != null )
                _universalVolume.SetActive( renderPipeline.Type == RenderPipelineType.Universal );

            if( _highDefinitionVolume != null )
                _highDefinitionVolume.SetActive( renderPipeline.Type == RenderPipelineType.HighDefinition );

            if( _standardLighting != null )
                _standardLighting.SetActive( renderPipeline.Type == RenderPipelineType.Standard );

            if( _universalLighting != null )
                _universalLighting.SetActive( renderPipeline.Type == RenderPipelineType.Universal );

            if( _highDefinitionLighting != null )
                _highDefinitionLighting.SetActive( renderPipeline.Type == RenderPipelineType.HighDefinition );

            switch( renderPipeline.Type )
            {
                case RenderPipelineType.Standard:
                    RenderSettings.skybox = _standardSky;
                    SetTerrainMaterial( _standardTerrain );
                    break;
                case RenderPipelineType.Universal:
                    RenderSettings.skybox = _universalSky;
                    SetTerrainMaterial( _universalTerrain );
                    break;
                case RenderPipelineType.HighDefinition:
                    RenderSettings.skybox = _highDefinitionSky;
                    SetTerrainMaterial( _highDefinitionTerrain );
                    break;
            }
#endif
        }

#if UNITY_EDITOR
        private void SetTerrainMaterial( Material material )
        {
            foreach( var terrain in FindObjectsOfType<Terrain>() )
                terrain.materialTemplate = material;
        }
#endif
    }
}