﻿using SingleTrainTrack.NEXT.Texturing;

namespace SingleTrainTrack.NEXT.Extensions
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo SetAllSegmentsTexture(this NetInfo info, TextureSet newTextures, LODTextureSet newLODTextures = null)
        {
            foreach (var segment in info.m_segments)
            {
                segment.SetTextures(newTextures, newLODTextures);
            }

            return info;
        }

        public static NetInfo.Segment SetTextures(this NetInfo.Segment segment, TextureSet newTextures, LODTextureSet newLODTextures = null)
        {
            if (newTextures != null)
            {
                if (segment.m_material != null)
                {
                    segment.m_material = newTextures.CreateRoadMaterial(segment.m_material);
                }
            }

            if (newLODTextures != null)
            {
                if (segment.m_lodMaterial != null)
                {
                    segment.m_lodMaterial = newLODTextures.CreateRoadMaterial(segment.m_lodMaterial);
                }
            }

            return segment;
        }

        public static NetInfo.Segment SetMeshes(this NetInfo.Segment segment, string newMeshPath, string newLODMeshPath = null)
        {
            segment.m_mesh = AssetManager.instance.GetMesh(newMeshPath);

            if (newLODMeshPath != null)
            {
                segment.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return segment;
        }


        public static NetInfo.Segment SetFlagsDefault(this NetInfo.Segment segment)
        {
            segment.m_backwardForbidden = NetSegment.Flags.None;
            segment.m_backwardRequired = NetSegment.Flags.None;

            segment.m_forwardForbidden = NetSegment.Flags.None;
            segment.m_forwardRequired = NetSegment.Flags.None;

            return segment;
        }
    }
}
