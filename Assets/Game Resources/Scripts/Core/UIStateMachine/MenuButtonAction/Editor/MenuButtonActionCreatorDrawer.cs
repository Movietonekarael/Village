using GameCore.Network;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameCore
{
    namespace GUI
    {
        [CustomPropertyDrawer(typeof(MenuButtonActionCreator))]
        public sealed class MenuButtonActionCreatorDrawer : PropertyDrawer
        {
            private struct PropertyInfo
            {
                public FieldInfo Field;
                public object PropertyValue;
                public int Index;

                public PropertyInfo(FieldInfo field, object propertyValue, int index)
                {
                    Field = field;
                    PropertyValue = propertyValue;
                    Index = index;
                }
            }

            private const string _CURRENT_ACTION_TYPE = "CurrentActionType";
            private const string _LAST_ACTION_TYPE = "LastActionType";

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUILayout.PropertyField(property.FindPropertyRelative(_CURRENT_ACTION_TYPE), GUIContent.none);

                var (creator, actionType) = ReplaceCreatorIfEnumChanged(property);
                DrawSpecificCreators(creator, actionType);

                EditorGUI.EndProperty();
            }

            private (MenuButtonActionCreator creator, MenuButtonActionCreator.ActionType actionType) ReplaceCreatorIfEnumChanged(SerializedProperty property)
            {
                var propertyInfo = GetFieldAndPropertyValue(property);

                var (currentActionType, lastActionType) = GetActionTypes(property, propertyInfo);
                
                if (currentActionType != lastActionType)
                {
                    var newCreator = Activator.CreateInstance(MenuButtonActionCreator.actionsList[(int)currentActionType]) as MenuButtonActionCreator;
                    if(!propertyInfo.Field.FieldType.IsArray)
                    {
                        propertyInfo.Field.SetValue(propertyInfo.PropertyValue, newCreator);
                    }
                    else
                    {
                        var creatorsArray = propertyInfo.Field.GetValue(propertyInfo.PropertyValue) as MenuButtonActionCreator[];
                        creatorsArray[propertyInfo.Index] = newCreator;
                    }
                    
                    newCreator.CurrentActionType = currentActionType;
                    newCreator.LastActionType = currentActionType;
                    return (newCreator, currentActionType);
                }

                var currentCreator = GetCreator(propertyInfo);
                return (currentCreator, currentActionType);
            }

            private (MenuButtonActionCreator.ActionType current, MenuButtonActionCreator.ActionType last) GetActionTypes(SerializedProperty property, PropertyInfo propertyInfo)
            {
                MenuButtonActionCreator.ActionType currentActionType;
                MenuButtonActionCreator.ActionType lastActionType;

                if (propertyInfo.Field.FieldType.IsArray)
                {
                    var creatorsArray = propertyInfo.Field.GetValue(propertyInfo.PropertyValue) as MenuButtonActionCreator[];
                    currentActionType = creatorsArray[propertyInfo.Index].CurrentActionType;
                    lastActionType = creatorsArray[propertyInfo.Index].LastActionType;
                }
                else
                {
                    currentActionType = (MenuButtonActionCreator.ActionType)property.FindPropertyRelative(_CURRENT_ACTION_TYPE).enumValueIndex;
                    lastActionType = (MenuButtonActionCreator.ActionType)property.FindPropertyRelative(_LAST_ACTION_TYPE).enumValueIndex;
                }

                return (currentActionType, lastActionType);
            }

            private MenuButtonActionCreator GetCreator(PropertyInfo propertyInfo)
            {
                if (!propertyInfo.Field.FieldType.IsArray)
                {
                    return propertyInfo.Field.GetValue(propertyInfo.PropertyValue) as MenuButtonActionCreator;
                }
                else
                {
                    var creatorsArray = propertyInfo.Field.GetValue(propertyInfo.PropertyValue) as MenuButtonActionCreator[];
                    return creatorsArray[propertyInfo.Index];
                }
            }

            private PropertyInfo GetFieldAndPropertyValue(SerializedProperty property)
            {
                var propertyValue = (object)property.serializedObject.targetObject;
                var path = property.propertyPath;
                var subPaths = path.Split('.');
                var type = propertyValue.GetType();
                var field = type.GetField(subPaths[0]);
                if (field.FieldType.IsArray)
                {
                    var lastSubPathIndex = subPaths.Length - 1;
                    var index = GetArrayIndexFromSubPath(subPaths[lastSubPathIndex]);
                    return new PropertyInfo(field, propertyValue, index);
                }
                else
                {
                    return new PropertyInfo(field, propertyValue, 0);
                }
            }

            private int GetArrayIndexFromSubPath(string subPath)
            {
                var startIndex = subPath.IndexOf('[');
                var endIndex = subPath.IndexOf(']');
                var indexString = subPath.Substring(startIndex + 1, endIndex - startIndex - 1);
                var index = Convert.ToInt32(indexString);
                return index;
            }

            private void DrawSpecificCreators(MenuButtonActionCreator creator, MenuButtonActionCreator.ActionType actionType)
            {
                switch (actionType)
                {
                    case MenuButtonActionCreator.ActionType.LoadScene:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Load scene name:");
                        var loadCreator = creator as LoadSceneButtonActionCreator;
                        loadCreator.LoadSceneName = EditorGUILayout.TextField(loadCreator.LoadSceneName);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case MenuButtonActionCreator.ActionType.SetConnectionType:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("ConnectionType:");
                        var connectionCreator = creator as SetConnectionTypeButtonActionCreator;
                        connectionCreator.ConnectionType = (ConnectionType)EditorGUILayout.EnumPopup(connectionCreator.ConnectionType);
                        EditorGUILayout.EndHorizontal();
                        break;
                }
            }
        }
    }
}