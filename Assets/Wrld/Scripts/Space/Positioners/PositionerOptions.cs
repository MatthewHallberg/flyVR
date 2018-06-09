namespace Wrld.Space.Positioners
{
    /// <summary>
    /// Defines creation parameters for a Positioner.
    /// </summary>
    public class PositionerOptions
    {
        private double m_latitudeDegrees;
        private double m_longitudeDegrees;
        private double m_elevation;
        private ElevationMode m_elevationMode = ElevationMode.HeightAboveGround;
        private string m_indoorMapId = "";
        private int m_indoorMapFloorId;
        private bool m_usingFloorId = false;

        public PositionerOptions()
        {
        }

        /// <summary>
        /// Sets the target Latitude of this Positioner.
        /// </summary>
        /// <param name="latitudeDegrees">The Latitude, in degrees, to move this Positioner to.</param>
        /// <returns>This PositionerOptions instance, with the target latitude set.</returns>
        public PositionerOptions LatitudeDegrees(double latitudeDegrees)
        {
            m_latitudeDegrees = latitudeDegrees;
            return this;
        }

        /// <summary>
        /// Sets the target Longitude of this Positioner.
        /// </summary>
        /// <param name="longitudeDegrees">The Longitude, in degrees, to move this Positioner to.</param>
        /// <returns>This PositionerOptions instance, with the target longitude set.</returns>
        public PositionerOptions LongitudeDegrees(double longitudeDegrees)
        {
            m_longitudeDegrees = longitudeDegrees;
            return this;
        }

        /// <summary>
        /// Sets the target Elevation of this Positioner, relative to the elevation of the terrain.
        /// </summary>
        /// <param name="elevation">The Elevation to move this Positioner to.</param>
        /// <returns>This PositionerOptions instance, with the target elevation set.</returns>
        public PositionerOptions ElevationAboveGround(double elevation)
        {
            m_elevation = elevation;
            m_elevationMode = ElevationMode.HeightAboveGround;
            return this;
        }

        /// <summary>
        /// Sets the target Elevation of this Positioner, relative to sea-level.
        /// </summary>
        /// <param name="elevation">The Elevation to move this Positioner to.</param>
        /// <returns>This PositionerOptions instance, with the target elevation set.</returns>
        public PositionerOptions ElevationAboveSeaLevel(double elevation)
        {
            m_elevation = elevation;
            m_elevationMode = ElevationMode.HeightAboveSeaLevel;
            return this;
        }

        /// <summary>
        /// Sets the Indoor Map of this Positioner, using an Indoor Map Id.
        /// </summary>
        /// <param name="indoorMapId">The ID of the Indoor Map to move this Positioner to.</param>
        /// <returns>This PositionerOptions instance, with the Indoor Map Id set.</returns>
        public PositionerOptions IndoorMap(string indoorMapId)
        {
            m_indoorMapId = indoorMapId;
            m_indoorMapFloorId = 0;
            m_usingFloorId = false;
            return this;
        }

        /// <summary>
        /// Sets the Indoor Map and current target Floor of this Positioner, using an Indoor Map Id and a Floor Id.
        /// </summary>
        /// <param name="indoorMapId">The ID of the Indoor Map to move this Positioner to.</param>
        /// <param name="indoorMapFloorId">The ID of the Floor to move this Positioner to.</param>
        /// <returns>This PositionerOptions instance, with the Indoor Map Id and Floor set.</returns>
        public PositionerOptions IndoorMapWithFloorId(string indoorMapId, int indoorMapFloorId)
        {
            m_indoorMapId = indoorMapId;
            m_indoorMapFloorId = indoorMapFloorId;
            m_usingFloorId = true;
            return this;
        }

        internal ElevationMode GetElevationMode()
        {
            return m_elevationMode;
        }

        internal double GetLatitudeDegrees()
        {
            return m_latitudeDegrees;
        }

        internal double GetLongitudeDegrees()
        {
            return m_longitudeDegrees;
        }

        internal double GetElevation()
        {
            return m_elevation;
        }

        internal string GetIndoorMapId()
        {
            return m_indoorMapId;
        }

        internal int GetIndoorMapFloorId()
        {
            return m_indoorMapFloorId;
        }

        internal bool IsUsingFloorId()
        {
            return m_usingFloorId;
        }
    }
}
