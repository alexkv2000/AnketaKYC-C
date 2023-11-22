
using System.Xml.Serialization;

namespace ExempleAnketaKYCService.DeserializeClasses
{
    /// <summary>
    /// Резултат ответа смены состояния
    /// </summary>
    public class ChangeStatusResponse 
    {

        /// <summary>
        /// Результат смены
        /// </summary>
        [XmlRoot("result")]
        public enum ChangeStatusEnum
        {
            [XmlEnum(Name = "0")]
            Error,
            [XmlEnum(Name = "1")]
            Ok
        }

    }

    


}
