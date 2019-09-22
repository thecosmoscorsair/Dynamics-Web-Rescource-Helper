namespace DynamicsJScriptHelper.Dynamics
{
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using static DynamicsJScriptHelper.Common.Logger;
    using static System.Convert;
    using static System.Text.Encoding;

    internal class WebResourceUpdate
    {
        private readonly string
            _name,
            _content;

        private readonly IOrganizationService
            _service;

        internal WebResourceUpdate(
            string name,
            string content,
            IOrganizationService service)
        {
            name = name
                .Substring(name.LastIndexOf("\\") + 1);
            _name = name
                .Substring(0, name.LastIndexOf(".")); //todo tidy this

            _content = ToBase64String(UTF8.GetBytes(content));
            _service = service;
        }

        internal bool UpdateWebResource()
        {
            var matchingResources = GetMatchingResources();

            if(matchingResources.Entities.Count == 0)
            {
                Log($"No web resources match the name \"{_name}\".", true);
                return false;
            }

            if (matchingResources.Entities.Count > 1)
            {
                Log($"{matchingResources.Entities.Count} web resources match {_name}.", true);
                return false;
            }

            PerformUpdate(matchingResources.Entities[0].ToEntityReference());

            return true;
        }

        internal static void PublishCustomisations(IOrganizationService service)
            => service.Execute(new PublishAllXmlRequest());

        private EntityCollection GetMatchingResources()
        {
            var query = new QueryExpression
            {
                EntityName = "webresource",
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(
                            "name",
                            ConditionOperator.Equal,
                            _name),
                    }
                },
            };
            return _service.RetrieveMultiple(query);
        }

        private void PerformUpdate(EntityReference webResourceRef)
        {
            var webResource = new Entity(
                webResourceRef.LogicalName,
                webResourceRef.Id);

            webResource["content"] = _content;

            _service.Update(webResource);
        }
    }
}
