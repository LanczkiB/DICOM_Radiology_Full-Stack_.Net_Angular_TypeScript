using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace DICOMweb.Entities
{
    
    public class Resource
    {
        public string Type;

        //required Matching Attribues
        public Tag StudyDate { get; set; }
        public Tag StudyTime { get; set; }
        public Tag AccessionNumber { get; set; }
        public Tag ModalitiesInStudy { get; set; }
        public Tag ReferringPhisicianName { get; set; }
        public Tag PatientName { get; set; }
        public Tag PatientID { get; set; }
        public Tag StudyInstanceUID { get; set; }
        public Tag StudyID { get; set; }
        public Tag InstanceAvailability { get; set; }
        public Tag Timezone { get; set; }
        public Tag RetrieveURL { get; set; }
        public Tag PatientBirthDate { get; set; }
        public Tag PatientsSex { get; set; }
        public Tag NumberOfStudyRelatedSeries { get; set; }
        public Tag NumberOfStudyRelatedInstances { get; set; }
        public Tag Modality { get; set; }
        public Tag SeriesInstanceUID { get; set; }
        public Tag SeriesNumber { get; set; }
        public Tag StepStartDate { get; set; }
        public Tag StepStartTime { get; set; }
        public Tag RequestAttributesSequence { get; set; }
        public Tag ScheduledProcedureStepID { get; set; }
        public Tag RequestedProcedureID { get; set; }
        public Tag SeriesDescription { get; set; }
        public Tag SopClassUID { get; set; }
        public Tag SopInstanceUID { get; set; }
        public Tag InstanceNumber { get; set; }
        public Tag Rows { get; set; }
        public Tag Columns { get; set; }
        public Tag BitsAllocated { get; set; }
        public Tag NumberOfFrames { get; set; }
        public Tag NumberOfSeriesRelatedInstances;
        public Tag SpecificCharacterSet { get; set; }


        public Resource()
        {
            PropertySetLooping();
        }

        void PropertySetLooping()
        {
            foreach (PropertyInfo info in this.GetType().GetRuntimeProperties())
            {
                info.SetValue(this,new Tag("Unknown"));
            }
        }

        public void SetResourceTag(string tag, string value, string vr)
        {
            switch (tag)
            {
                case "00080020": { StudyDate = new Tag("StudyDate", tag, value, vr); break; }
                case "00080030": { StudyTime = new Tag("StudyTime", tag, value, vr); break; }
                case "00080050": { AccessionNumber = new Tag("AccessionNumber", tag, value, vr); break; }
                case "00080061": { ModalitiesInStudy = new Tag("ModalitiesInStudy", tag, value, vr); break; }
                case "00080090": { ReferringPhisicianName = new Tag("ReferringPhisicianName", tag, value, vr); break; }
                case "00100010": { PatientName = new Tag("PatientName", tag, value, vr); break; }
                case "00100020": { PatientID = new Tag("PatientID", tag, value, vr); break; }
                case "0020000D": { StudyInstanceUID = new Tag("StudyInstanceUID", tag, value, vr); Type = "Study"; break; }
                case "00200010": { StudyID = new Tag("StudyID", tag, value, vr); break; }
                case "00080056": { InstanceAvailability = new Tag("InstanceAvailability", tag, value, vr); break; }
                case "00080201": { Timezone = new Tag("Timezone", tag, value, vr); break; }
                case "00081190": { RetrieveURL = new Tag("RetrieveURL", tag, value, vr); break; }
                case "00100030": { PatientBirthDate = new Tag("PatientBirthDate", tag, value, vr); break; }
                case "00100040": { PatientsSex = new Tag("PatientsSex", tag, value, vr); break; }
                case "00201206": { NumberOfStudyRelatedSeries = new Tag("NumberOfSeries", tag, value, vr); break; }
                case "00201208": { NumberOfStudyRelatedInstances = new Tag("NumberOfInstances", tag, value, vr); break; }
                case "00080060": { Modality = new Tag("Modality", tag, value, vr); break; }
                case "0020000E": { SeriesInstanceUID = new Tag("SeriesInstanceUID", tag, value, vr); Type = "Series"; break; }
                case "00200011": { SeriesNumber = new Tag("SeriesNumber", tag, value, vr); break; }
                case "00400244": { StepStartDate = new Tag("StepStartDate", tag, value, vr); break; }
                case "0008103E": { SeriesDescription = new Tag("SeriesDescription", tag, value, vr); break; }
                case "00400245": { StepStartTime = new Tag("StepStartTime", tag, value, vr); break; }
                case "00400275": { RequestAttributesSequence = new Tag("RequestAttributesSequence", tag, value, vr); break; }
                case "00400009": { ScheduledProcedureStepID = new Tag("ScheduledProcedureStepID", tag, value, vr); break; }
                case "00401001": { RequestedProcedureID = new Tag("RequestedProcedureID", tag, value, vr); break; }
                case "00080016": { SopClassUID = new Tag("SOPClassUID", tag, value, vr); break; }
                case "00080018": { SopInstanceUID = new Tag("SOPInstanceUID", tag, value, vr); Type = "Instance"; break; }
                case "00200013": { InstanceNumber = new Tag("InstanceNumber", tag, value, vr); break; }
                case "00280010": { Rows = new Tag("Rows", tag, value, vr); break; }
                case "00280011": { Columns = new Tag("Columns", tag, value, vr); break; }
                case "00280100": { BitsAllocated = new Tag("BitsAllocated", tag, value, vr); break; }
                case "00280008": { NumberOfFrames = new Tag("NumberOfFrames", tag, value, vr); break; }
                case "00201209": { NumberOfSeriesRelatedInstances = new Tag("NumberOfSeriesRelatedInstances", tag, value, vr); break; }
                case "00080005": { SpecificCharacterSet = new Tag("SpecificCharacterSet", tag, value, vr); break; }
                default: { Console.WriteLine("New tag " + tag);
                        Logger.LogStringWarning("New tag discovered while parsing HTTP GET response: "+tag);
                        break; }
            }
        }
    }
}
