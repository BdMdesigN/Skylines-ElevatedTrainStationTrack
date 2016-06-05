﻿using System;
using System.Linq;
using Transit.Framework;
using Transit.Framework.Builders;
using Transit.Addon.RoadExtensions.PublicTransport.RailUtils;
using System.Collections.Generic;
using Transit.Framework.Network;
using Transit.Addon.RoadExtensions.Roads.Common;
using UnityEngine;

namespace Transit.Addon.RoadExtensions.PublicTransport.Rail1L
{
    public partial class Rail1LBuilder : Activable, INetInfoBuilderPart, INetInfoLateBuilder
    {
        public int Order { get { return 7; } }
        public int UIOrder { get { return 9; } }

        public string BasedPrefabName { get { return "Train Track"; } }
        public string Name { get { return "Rail1L"; } }
        public string DisplayName { get { return "Single Rail Track"; } }
        public string Description { get { return "A single one way rail track that can be connected to conventional rail."; } }
        public string ShortDescription { get { return "Single Rail Track"; } }
        public string UICategory { get { return "PublicTransportTrain"; } }

        public string ThumbnailsPath { get { return @"Textures\Rail1L\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Textures\Rail1L\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            //var highwayInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.HIGHWAY_3L_SLOPE);
            var railVersionName = string.Format("{0} {1}", "Train Track", (version == NetInfoVersion.Ground ? string.Empty : version.ToString())).Trim();
            var railInfo = Prefabs.Find<NetInfo>(railVersionName);
            UnityEngine.Debug.Log("this is " + railVersionName);
            //var owRoadTunnelInfo = Prefabs.Find<NetInfo>(NetInfos.Vanilla.ONEWAY_2L_TUNNEL);
            info.m_class = railInfo.m_class.Clone("NExtSingleTrack");
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup10mMesh(version);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            //info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            if (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel)
            {
                info.m_halfWidth = 4;
                info.m_pavementWidth = 2;
            }
            else
            {
                info.m_halfWidth = 3;
            }
            
            if (version == NetInfoVersion.Tunnel)
            {
                info.m_setVehicleFlags = Vehicle.Flags.Transition;
                info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
                //info.m_class = owRoadTunnelInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD_TUNNEL);
            }
            else
            {
                //info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            }

            //var propLanes = info.m_lanes.Where(l => l.m_laneProps != null && (l.m_laneProps.name.ToLower().Contains("left") || l.m_laneProps.name.ToLower().Contains("right"))).ToList();

            //var remainingLanes = new List<NetInfo.Lane>();
            //remainingLanes.AddRange(info
            //    .m_lanes
            //    .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian || l.m_laneType == NetInfo.LaneType.None || l.m_laneType == NetInfo.LaneType.Parking));
            //remainingLanes.AddRange(info
            //    .m_lanes
            //    .Where(l => l.m_laneType != NetInfo.LaneType.None)
            //    .Skip(1));
            //info.m_lanes = remainingLanes.ToArray();
            info.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = false,
                LanesToAdd = -1,
            });

            //info.m_class.m_layer = ItemClass.Layer.PublicTransport;
            info.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
            info.m_nodeConnectGroups = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.NarrowTram;
            //info.m_nodes[1].m_connectGroup = (NetInfo.ConnectGroup)9; 
            var owPlayerNetAI = railInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();
            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 3 / 2; 
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 3 / 2; 
            }

            var trainTrackAI = info.GetComponent<TrainTrackAI>();

            if (trainTrackAI != null)
            {

            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            var plPropInfo = PrefabCollection<PropInfo>.FindLoaded("Rail1LPowerLine.Rail1LPowerLine_Data");
            if (plPropInfo == null)
            {
                plPropInfo = PrefabCollection<PropInfo>.FindLoaded("478820060.Rail1LPowerLine_Data");
            }
            var oldPlPropInfo = Prefabs.Find<PropInfo>("RailwayPowerline");
            OneWayTrainTrack.NetInfoExtensions.ReplaceProps(info, plPropInfo, oldPlPropInfo);
            for (int i = 0; i < info.m_lanes.Count(); i++)
            {
                var powerLineProp = info.m_lanes[i].m_laneProps.m_props.Where(p => p.m_prop == plPropInfo).ToList();
                for (int j = 0; j < powerLineProp.Count(); j++)
                {
                    powerLineProp[j].m_position = new Vector3(2.4f, -0.15f, 0);
                    powerLineProp[j].m_angle = 180;
                }
            }

            if (version == NetInfoVersion.Elevated)
            {
                var epPropInfo = PrefabCollection<BuildingInfo>.FindLoaded("478820060.Rail1LElevatedPillar_Data");

                if (epPropInfo == null)
                {
                    epPropInfo = PrefabCollection<BuildingInfo>.FindLoaded("Rail1LElevatedPillar.Rail1LElevatedPillar_Data");
                }

                if (epPropInfo != null)
                {
                    var bridgeAI = info.GetComponent<TrainTrackBridgeAI>();
                    if (bridgeAI != null)
                    {
                        bridgeAI.m_doubleLength = false;
                        bridgeAI.m_bridgePillarInfo = epPropInfo;
                        bridgeAI.m_bridgePillarOffset = 1;
                        bridgeAI.m_middlePillarInfo = null;
                    }
                }
            }
            else if (version == NetInfoVersion.Bridge)
            {
                var bpPropInfo = PrefabCollection<BuildingInfo>.FindLoaded("478820060.Rail1LBridgePillar_Data");

                if (bpPropInfo == null)
                {
                    bpPropInfo = PrefabCollection<BuildingInfo>.FindLoaded("Rail1LBridgePillar.Rail1LBridgePillar_Data");
                }

                if (bpPropInfo != null)
                {
                    var bridgeAI = info.GetComponent<TrainTrackBridgeAI>();
                    if (bridgeAI != null)
                    {
                        //bridgeAI.m_doubleLength = false;
                        bridgeAI.m_bridgePillarInfo = bpPropInfo;
                        //bridgeAI.m_bridgePillarOffset = 1;
                        //bridgeAI.m_middlePillarInfo = null;
                    }
                }
            }
        }
    }
}
