using System.Collections.Generic;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Extension.Interface;
using inRiver.Remoting.Objects;

namespace Inriver.CognitiveServices
{
    public class CognitiveServicesExtensionListener : IEntityListener
    {
        public string Test()
        {
            return "ok";
        }

        public void EntityCreated(int entityId)
        {
            new ImageAnalyzerService(Context).AnalyzeImage(entityId);
        }

        #region Unused
        public void EntityUpdated(int entityId, string[] fields)
        {
        }

        public void EntityDeleted(Entity deletedEntity)
        {
        }

        public void EntityLocked(int entityId)
        {
        }

        public void EntityUnlocked(int entityId)
        {
        }

        public void EntityFieldSetUpdated(int entityId, string fieldSetId)
        {
        }

        public void EntityCommentAdded(int entityId, int commentId)
        {
        }

        public void EntitySpecificationFieldAdded(int entityId, string fieldName)
        {
        }

        public void EntitySpecificationFieldUpdated(int entityId, string fieldName)
        {
        }
        #endregion

        public inRiverContext Context { get; set; }
        public Dictionary<string, string> DefaultSettings { get; } = new Dictionary<string, string>();
    }
}