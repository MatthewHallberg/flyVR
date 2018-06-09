using System;
using UnityEngine;
using Wrld.Common.Maths;

namespace Wrld.Space.Positioners
{
    /// <summary>
    /// A Positioner represents a single point on the map.
    /// </summary>
    public class Positioner
    {
        /// <summary>
        /// Uniquely identifies this object instance.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// This delegate is called when this Positioner's position is changed or updated. 
        /// </summary>
        public Action OnPositionerPositionChangedDelegate;

        private static int InvalidId = 0;

        private PositionerApiInternal m_positionerApiInternal;

        private LatLong m_position;
        private double m_elevation;
        private ElevationMode m_elevationMode;
        private string m_indoorMapId;
        private int m_indoorMapFloorId;


        // Use Api.Instance.PositionerApi.CreatePositioner for public construction
        internal Positioner(
            PositionerApiInternal positionerApiInternal,
            int id,
            ElevationMode elevationMode)
            {
                if (positionerApiInternal == null)
                {
                    throw new ArgumentNullException("positionerApiInternal");
                }

                if (id == InvalidId)
                {
                    throw new ArgumentException("invalid id");
                }

                m_positionerApiInternal = positionerApiInternal;
                Id = id;
                m_elevationMode = elevationMode;
            }

        /// <summary>
        /// Try to get the location of this Positioner, in ECEF space.
        /// </summary>
        /// <param name="out_positionerECEFLocation">The ECEF location of this Positioner. The value is only valid if the returned result is true.</param>
        /// <returns>True if the Positioner's ECEF location could be determined, false otherwise.</returns>
        public bool TryGetECEFLocation(out DoubleVector3 out_positionerECEFLocation)
        {
            if(m_positionerApiInternal.TryFetchECEFLocationForPositioner(this, out out_positionerECEFLocation))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to get the on-screen position of this Positioner. 
        /// </summary>
        /// <param name="out_screenPoint">The screen point of this Positioner. The value is only valid if the returned result is true.</param>
        /// <returns>True if the Positioner's screen point could be determined, false otherwise.</returns>
        public bool TryGetScreenPoint(out Vector3 out_screenPoint)
        {
            if (m_positionerApiInternal.TryFetchScreenPointForPositioner(this, out out_screenPoint))
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Set the location of this Positioner, at the specified latitude and longitude.
        /// </summary>
        /// <param name="latitudeDegrees">The desired latitude, in degrees.</param>
        /// <param name="longitudeDegrees">The desired longitude, in degrees.</param>
        [Obsolete("Please use Positioner.SetPosition(LatLong position) in the future.")]
        public void SetLocation(double latitudeDegrees, double longitudeDegrees)
        {
            m_position = LatLong.FromDegrees(latitudeDegrees, longitudeDegrees);
            SetPosition(m_position);
        }

        /// <summary>
        /// Set the location of this Positioner, at the specified latitude and longitude.
        /// </summary>
        /// <param name="position">The desired position in LatLong form.</param>
        public void SetPosition(LatLong position)
        {
            m_position = position;
            m_positionerApiInternal.SetPositionerLocation(this, m_position.GetLatitude(), m_position.GetLongitude());
        }

        /// <summary>
        /// Get the target latitude and longitude of this Positioner. Note: This is not the same as TryGetLatLongAltitude.
        /// </summary>
        /// <returns>The target position in LatLong form.</returns>
        public LatLong GetPosition()
        {
            return m_position;
        }

        /// <summary>
        /// Set the elevation of this Positioner, in meters. The behaviour of this depends on the ElevationMode.
        /// </summary>
        /// <param name="elevation">The desired elevation, in meters.</param>
        public void SetElevation(double elevation)
        {
            m_positionerApiInternal.SetPositionerElevation(this, elevation);
            m_elevation = elevation;
        }

        /// <summary>
        /// Get the elevation of this Positioner, in meters.
        /// </summary>
        /// <returns>The elevation of this Positioner, in meters.</returns>
        public double GetElevation()
        {
            return m_elevation;
        }

        /// <summary>
        /// Set the ElevationMode of this Positioner. See the ElevationMode documentation for more details.
        /// </summary>
        /// <param name="elevationMode">The desired ElevationMode of this positioner.</param>
        public void SetElevationMode(ElevationMode elevationMode)
        {
            m_positionerApiInternal.SetPositionerElevationMode(this, elevationMode);
            m_elevationMode = elevationMode;
        }

        /// <summary>
        /// Get the ElevationMode of this Positioner.
        /// </summary>
        /// <returns>The ElevationMode of this Positioner.</returns>
        public ElevationMode GetElevationMode()
        {
            return m_elevationMode;
        }

        /// <summary>
        /// Sets the Indoor Map of this Positioner. If this is unset, the Positioner will be outside instead.
        /// </summary>
        /// <param name="indoorMapId">The Indoor Map id string for the desired Indoor Map. See the IndoorMapApi documentation for more details.</param>
        /// <param name="indoorMapFloorId">The floor of the Indoor Map that this Positioner should be placed upon.</param>
        public void SetIndoorMap(string indoorMapId, int indoorMapFloorId)
        {
            m_positionerApiInternal.SetPositionerIndoorMap(this, indoorMapId, indoorMapFloorId);
            m_indoorMapId = indoorMapId;
            m_indoorMapFloorId = indoorMapFloorId;
        }

        /// <summary>
        /// Get the Indoor Map Id string of this Positioner.
        /// </summary>
        /// <returns>The Indoor Map Id, as a string.</returns>
        public string GetIndoorMapId()
        {
            return m_indoorMapId;
        }

        /// <summary>
        /// Get the Indoor Map Floor Id of this Positioner.
        /// </summary>
        /// <returns>The Indoor Map Floor Id of this Positioner.</returns>
        public int GetIndoorMapFloorId()
        {
            return m_indoorMapFloorId;
        }

        /// <summary>
        /// Try to get the transformed position as a LatLongAltitude of this Positioner. This can be used with SpacesApi.GeographicToWorldPoint to calculate a Vector3 translation for this Positioner. It is recommended to use a GeographicTransform for placing GameObjects on the map, however.
        /// </summary>
        /// <param name="out_latLongAlt">The LatLongAltitude that represents the Positioner's position with the desired elevation and ElevationMode applied. The value is only valid if this function returns true.</param>
        /// <returns>Whether or not this function was successful.</returns>
        public bool TryGetLatLongAltitude(out LatLongAltitude out_latLongAlt)
        {
            if (m_positionerApiInternal.TryFetchLatLongAltitudeForPositioner(this, out out_latLongAlt))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the screen projection of this Positioner would appear beyond the horizon for the current viewpoint. For example, when viewing the map zoomed out so that the entire globe is visible, calling this method on a Positioner that is located on the opposite side of the Earth from the camera would return true. 
        /// </summary>
        /// <returns>Whether or not this Positioner is beyond the horizon.</returns>
        public bool IsBehindGlobeHorizon()
        {
            return m_positionerApiInternal.IsPositionerBehindGlobeHorizon(this);
        }

        /// <summary>
        /// Destroys the Positioner.
        /// </summary>
        public void Discard()
        {
            m_positionerApiInternal.DestroyPositioner(this);
            InvalidateId();
        }

        private void InvalidateId()
        {
            Id = InvalidId;
        }
    }
}
