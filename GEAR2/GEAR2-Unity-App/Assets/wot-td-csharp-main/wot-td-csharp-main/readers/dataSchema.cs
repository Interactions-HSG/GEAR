using VDS.RDF;
using VDS.RDF.Parsing;
using System.Dynamic;
using System;
using System.Collections.Generic;

namespace wot_td_csharp
{
    public partial class TDGraphReader
    {
        DataSchema? ReadDataSchema(INode propertyNode, bool propertyAffordance)
        {
            // reading dataSchema from uriVariables is not recognized as a dataSchema from the RDF library - workaround
            DataSchemaType? dataType = ReadDataType(graph, propertyNode);
            if (dataType == null)
            {
                MyDataSchema.Builder builder = new MyDataSchema.Builder(propertyAffordance);
                AddDataSchemaProperties(builder, propertyNode);
                return builder.Build();
            }
            else if (dataType == DataSchemaType.array)
            {
                ArraySchema.Builder builder = new ArraySchema.Builder(propertyAffordance);
                builder.AddDataType(dataType.Value);
                AddDataSchemaProperties(builder, propertyNode);
                AddArraySchemaProperties(builder, propertyNode);
                return builder.Build();
            }
            else if (dataType == DataSchemaType.boolean)
            {
                BooleanSchema.Builder builder = new BooleanSchema.Builder(propertyAffordance);
                builder.AddDataType(dataType.Value);
                AddDataSchemaProperties(builder, propertyNode);
                return builder.Build();
            }
            else if (dataType == DataSchemaType.number)
            {
                NumberSchema.Builder builder = new NumberSchema.Builder(propertyAffordance);
                builder.AddDataType(dataType.Value);
                AddDataSchemaProperties(builder, propertyNode);
                AddNumberSchemaProperties(builder, propertyNode);
                return builder.Build();
            }
            else if (dataType == DataSchemaType.integer)
            {
                IntegerSchema.Builder builder = new IntegerSchema.Builder(propertyAffordance);
                builder.AddDataType(dataType.Value);
                AddDataSchemaProperties(builder, propertyNode);
                AddIntegerSchemaProperties(builder, propertyNode);
                return builder.Build();
            }
            else if (dataType == DataSchemaType.@object)
            {
                ObjectSchema.Builder builder = new ObjectSchema.Builder(propertyAffordance);
                builder.AddDataType(dataType.Value);
                AddDataSchemaProperties(builder, propertyNode);
                AddObjectSchemaProperties(builder, propertyNode);
                return builder.Build();
            }
            else if (dataType == DataSchemaType.@string)
            {
                StringSchema.Builder builder = new StringSchema.Builder(propertyAffordance);
                builder.AddDataType(dataType.Value);
                AddDataSchemaProperties(builder, propertyNode);
                AddStringSchemaProperties(builder, propertyNode);
                return builder.Build();
            }
            else if (dataType == DataSchemaType.@null)
            {
                NullSchema.Builder builder = new NullSchema.Builder();
                return builder.Build();
            }
            return null;
        }

        DataSchemaType? ReadDataType(Graph graph, INode propertyNode)
        {
            string? RDFTypeString = Utils
                .GetObjectNode(graph, propertyNode, RdfSpecsHelper.RdfType)
                ?.ToString();
            if (RDFTypeString != null)
            {
                if (RDFTypeString.Equals(JSONSchema.ArraySchema))
                    return DataSchemaType.array;
                else if (RDFTypeString.Equals(JSONSchema.BooleanSchema))
                    return DataSchemaType.boolean;
                else if (RDFTypeString.Equals(JSONSchema.NumberSchema))
                    return DataSchemaType.number;
                else if (RDFTypeString.Equals(JSONSchema.IntegerSchema))
                    return DataSchemaType.integer;
                else if (RDFTypeString.Equals(JSONSchema.ObjectSchema))
                    return DataSchemaType.@object;
                else if (RDFTypeString.Equals(JSONSchema.StringSchema))
                    return DataSchemaType.@string;
                else if (RDFTypeString.Equals(JSONSchema.NullSchema))
                    return DataSchemaType.@null;
            }

            return null;
        }

        void AddDataSchemaProperties<T, S>(DataSchema.Builder<T, S> builder, INode propertyNode)
            where T : DataSchema
            where S : DataSchema.Builder<T, S>
        {
            string? title = Utils.GetObjectNode(graph, propertyNode, TD.title)?.ToString();
            string? description = Utils.GetObjectNode(graph, propertyNode, TD.description)?.ToString();
            dynamic? defaultValue = ReadDefaultValue(propertyNode);
            INode? unitNode = Utils.GetObjectNode(graph, propertyNode, "http://schema.org/unitCode");
            string? unit = null;
            if (unitNode != null)
                unit = Utils.ParseNodeName(unitNode.ToString());
            List<DataSchema>? oneOf = ReadOneOf(propertyNode);
            List<dynamic>? enumValues = ReadEnumValues(propertyNode);
            bool? readOnly = Utils.GetObjectBoolean(graph, propertyNode, JSONSchema.readOnly);
            bool? writeOnly = Utils.GetObjectBoolean(graph, propertyNode, JSONSchema.writeOnly);
            string? format = Utils.GetObjectName(graph, propertyNode, JSONSchema.format);

            if (title != null)
                builder.AddTitle(title);
            if (description != null)
                builder.AddDescription(description);
            if (defaultValue != null)
                builder.AddDefault(defaultValue);
            if (unit != null)
                builder.AddUnit(unit);
            if (oneOf != null)
                builder.AddOneOfs(oneOf);
            if (enumValues != null)
                builder.AddEnum(enumValues);
            if (readOnly != null)
                builder.AddReadOnly(readOnly.Value);
            if (writeOnly != null)
                builder.AddWriteOnly(writeOnly.Value);
            if (format != null)
                builder.AddFormat(format);
        }

        void AddArraySchemaProperties(ArraySchema.Builder builder, INode propertyNode)
        {
            List<DataSchema> items = new List<DataSchema>();
            // check if the Schema has a properties node
            IEnumerable<Triple> itemsTriples = graph.GetTriplesWithSubjectPredicate(
                propertyNode,
                graph.CreateUriNode(new Uri(JSONSchema.items))
            );
            // add properties to the schema
            foreach (Triple itemsTriple in itemsTriples)
            {
                DataSchema? readDataSchema = ReadDataSchema(itemsTriple.Object, false);

                if (readDataSchema != null)
                {
                    items.Add(readDataSchema);
                }
            }

            int? minItems = Utils.GetObjectInt(graph, propertyNode, JSONSchema.minItems);
            int? maxItems = Utils.GetObjectInt(graph, propertyNode, JSONSchema.maxItems);

            if (items.Count > 0)
                builder.AddItems(items);
            if (minItems != null)
                builder.AddMinItems(minItems.Value);
            if (maxItems != null)
                builder.AddMaxItems(maxItems.Value);
        }

        void AddNumberSchemaProperties(NumberSchema.Builder builder, INode propertyNode)
        {
            double? minimum = Utils.GetObjectFloat(graph, propertyNode, JSONSchema.minimum);
            double? exclusiveMinimum = Utils.GetObjectFloat(
                graph,
                propertyNode,
                JSONSchema.exclusiveMinimum
            );
            double? maximum = Utils.GetObjectFloat(graph, propertyNode, JSONSchema.maximum);
            double? exclusiveMaximum = Utils.GetObjectFloat(
                graph,
                propertyNode,
                JSONSchema.exclusiveMaximum
            );
            double? multipleOf = Utils.GetObjectFloat(graph, propertyNode, JSONSchema.multipleOf);

            if (minimum != null)
                builder.AddMinimum(minimum.Value);
            if (exclusiveMinimum != null)
                builder.AddExclusiveMinimum(exclusiveMinimum.Value);
            if (maximum != null)
                builder.AddMaximum(maximum.Value);
            if (exclusiveMaximum != null)
                builder.AddExclusiveMaximum(exclusiveMaximum.Value);
            if (multipleOf != null)
                builder.AddMultipleOf(multipleOf.Value);
        }

        void AddIntegerSchemaProperties(IntegerSchema.Builder builder, INode propertyNode)
        {
            int? minimum = Utils.GetObjectInt(graph, propertyNode, JSONSchema.minimum);
            int? exclusiveMinimum = Utils.GetObjectInt(
                graph,
                propertyNode,
                JSONSchema.exclusiveMinimum
            );
            int? maximum = Utils.GetObjectInt(graph, propertyNode, JSONSchema.maximum);
            int? exclusiveMaximum = Utils.GetObjectInt(
                graph,
                propertyNode,
                JSONSchema.exclusiveMaximum
            );
            int? multipleOf = Utils.GetObjectInt(graph, propertyNode, JSONSchema.multipleOf);

            if (minimum != null)
                builder.AddMinimum(minimum.Value);
            if (exclusiveMinimum != null)
                builder.AddExclusiveMinimum(exclusiveMinimum.Value);
            if (maximum != null)
                builder.AddMaximum(maximum.Value);
            if (exclusiveMaximum != null)
                builder.AddExclusiveMaximum(exclusiveMaximum.Value);
            if (multipleOf != null)
                builder.AddMultipleOf(multipleOf.Value);
        }

        void AddObjectSchemaProperties(ObjectSchema.Builder builder, INode propertyNode)
        {
            // read properties
            Dictionary<string, DataSchema> properties = new Dictionary<string, DataSchema>();
            // check if the Schema has a properties node
            IEnumerable<Triple> propertiesTriples = graph.GetTriplesWithSubjectPredicate(
                propertyNode,
                graph.CreateUriNode(new Uri(JSONSchema.properties))
            );

            // add properties to the schema
            foreach (Triple propertiesTriple in propertiesTriples)
            {
                INode schemaNode = propertiesTriple.Object;
                string schemaName;

                string? schemaName1 =
                    Utils.GetObjectName(graph, schemaNode, TD.name);
                string? schemaName2 =
                    Utils.GetObjectName(graph, schemaNode, JSONSchema.propertyName);
                if (schemaName1 != null)
                    schemaName = schemaName1;
                else if (schemaName2 != null)
                    schemaName = schemaName2;
                else
                    throw new Exception("mandatory schema type name not found");

                DataSchema? dataSchema = ReadDataSchema(schemaNode, false);

                if (dataSchema != null)
                {
                    properties.Add(schemaName, dataSchema);
                }
            }

            // read required properties
            List<string> required = new List<string>();
            IEnumerable<Triple> requiredTriples = graph.GetTriplesWithSubjectPredicate(
                propertyNode,
                graph.CreateUriNode(new Uri(JSONSchema.required))
            );

            foreach (Triple requiredTriple in requiredTriples)
            {
                string requiredValue = Utils.ParseLiteralValue(requiredTriple.Object.ToString());
                required.Add(requiredValue);
            }

            if (properties.Count > 0)
                builder.AddProperties(properties);
            if (required.Count > 0)
                builder.AddRequired(required);
        }

        void AddStringSchemaProperties(StringSchema.Builder builder, INode propertyNode)
        {
            int? minLength = Utils.GetObjectInt(graph, propertyNode, JSONSchema.minLength);
            int? maxLength = Utils.GetObjectInt(graph, propertyNode, JSONSchema.maxLength);
            string? pattern = Utils.GetObjectName(graph, propertyNode, JSONSchema.pattern);
            string? contentEncoding = Utils.GetObjectName(
                graph,
                propertyNode,
                JSONSchema.contentMediaType
            );
            string? contentMediaType = Utils.GetObjectName(
                graph,
                propertyNode,
                JSONSchema.contentMediaType
            );

            if (minLength != null)
                builder.AddMinLength(minLength.Value);
            if (maxLength != null)
                builder.AddMaxLength(maxLength.Value);
            if (pattern != null)
                builder.AddPattern(pattern);
            if (contentEncoding != null)
                builder.AddContentEncoding(contentEncoding);
            if (contentMediaType != null)
                builder.AddContentMediaType(contentMediaType);
        }

        List<dynamic>? ReadEnumValues(INode propertyNode)
        {
            List<dynamic> enumValues = new List<dynamic>();
            IEnumerable<Triple> enumTriples = graph.GetTriplesWithSubjectPredicate(
                propertyNode,
                graph.CreateUriNode(new Uri(JSONSchema.enumeration))
            );

            foreach (Triple enumTriple in enumTriples)
            {
                dynamic enumValue = ParseDynamicValue(graph, enumTriple.Object);
                enumValues.Add(enumValue);
            }
            if (enumValues.Count == 0)
                return null;
            return enumValues;
        }

        dynamic? ReadDefaultValue(INode propertyNode)
        {
            INode? defaultValueNode = Utils.GetObjectNode(graph, propertyNode, JSONSchema.default_);

            if (defaultValueNode == null)
                return null;

            return ParseDynamicValue(graph, defaultValueNode);
        }

        dynamic ParseDynamicValue(Graph graph, INode dynamicNode)
        {
            dynamic dynamicValue = new System.Dynamic.ExpandoObject();
            if (dynamicNode is IBlankNode)
            {
                // get all triples
                IEnumerable<Triple> defaultValueTriples = graph.GetTriplesWithSubject(dynamicNode);
                foreach (Triple defaultValueTriple in defaultValueTriples)
                {
                    string predicate = defaultValueTriple.Predicate.ToString();
                    string name = Utils.ParseNodeName(predicate);

                    if (defaultValueTriple.Object is IBlankNode)
                    {
                        dynamic? value = ParseDynamicValue(graph, defaultValueTriple.Object);
                        AddDynamicValue(dynamicValue, name, value);
                    }
                    else
                    {
                        string objectString = defaultValueTriple.Object.ToString();
                        string objectLiteral = Utils.ParseLiteralValue(objectString);
                        DataSchemaType dataType = Utils.ParseNodeNameToDataType(objectString);
                        if (dataType == DataSchemaType.@string)
                            AddDynamicValue(dynamicValue, name, objectLiteral);
                        else if (dataType == DataSchemaType.number)
                            AddDynamicValue(dynamicValue, name, float.Parse(objectLiteral));
                        else if (dataType == DataSchemaType.integer)
                            AddDynamicValue(dynamicValue, name, int.Parse(objectLiteral));
                        else if (dataType == DataSchemaType.boolean)
                            AddDynamicValue(dynamicValue, name, bool.Parse(objectLiteral));
                        else if (dataType == DataSchemaType.@null)
                            AddDynamicValue(dynamicValue, name, null);
                    }
                }
            }
            else
            {
                string objectString = dynamicNode.ToString();
                string objectLiteral = Utils.ParseLiteralValue(objectString);
                DataSchemaType dataType = Utils.ParseNodeNameToDataType(objectString);
                if (dataType == DataSchemaType.@string)
                    dynamicValue = objectLiteral;
                else if (dataType == DataSchemaType.number)
                    dynamicValue = float.Parse(objectLiteral);
                else if (dataType == DataSchemaType.integer)
                    dynamicValue = int.Parse(objectLiteral);
                else if (dataType == DataSchemaType.boolean)
                    dynamicValue = bool.Parse(objectLiteral);
                // ???
                // else if (dataType == DataSchemaType.null_)
                //     dynamicValue = null;
            }

            return dynamicValue;
        }

        void AddDynamicValue(ExpandoObject dynamicValue, string name, dynamic value)
        {
            IDictionary<string, object> valueDictionary = dynamicValue;
            if (valueDictionary.ContainsKey(name))
            {
                List<dynamic> array = new List<dynamic>();
                if (valueDictionary[name] is List<dynamic> list)
                {
                    array = list;
                }
                else
                {
                    array.Add(valueDictionary[name]);
                }
                array.Add(value);
                valueDictionary[name] = array;
            }
            else
            {
                valueDictionary[name] = value;
            }
        }

        List<DataSchema>? ReadOneOf(INode propertyNode)
        {
            List<DataSchema> oneOf = new List<DataSchema>();
            // check if the Schema has a properties node
            IEnumerable<Triple> oneOfTriples = graph.GetTriplesWithSubjectPredicate(
                propertyNode,
                graph.CreateUriNode(new Uri(JSONSchema.oneOf))
            );

            // add properties to the schema
            foreach (Triple oneOfTriple in oneOfTriples)
            {
                INode schemaNode = oneOfTriple.Object;

                DataSchema? readDataSchema = ReadDataSchema(schemaNode, false);

                if (readDataSchema != null)
                {
                    oneOf.Add(readDataSchema);
                }
            }
            if (oneOf.Count == 0)
                return null;
            return oneOf;
        }
    }
}