using System;
using System.Collections.Generic;

namespace CaseNumbersWorker.Models
{
    public class UniqueIdField
    {
        public string name { get; set; }
        public bool isSystemMaintained { get; set; }
    }

    public class SpatialReference
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }
    }

    public class Field
    {
        public string name { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
        public string sqlType { get; set; }
        public object domain { get; set; }
        public object defaultValue { get; set; }
    }

    public class Attributes
    {
        public long Date_max { get; set; }
        public int ConfirmedCovidCases_sum { get; set; }
    }

    public class Feature
    {
        public Attributes attributes { get; set; }
    }

    public class ArcGisResponse
    {
        public string objectIdFieldName { get; set; }
        public UniqueIdField uniqueIdField { get; set; }
        public string globalIdFieldName { get; set; }
        public string geometryType { get; set; }
        public SpatialReference spatialReference { get; set; }
        public IList<Field> fields { get; set; }
        public IList<Feature> features { get; set; }
    }
}
