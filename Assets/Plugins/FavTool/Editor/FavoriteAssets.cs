using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.IO;
using System.Linq;
using System;

namespace FavTool.FavoriteAssets
{
    public class FavoriteAssets : EditorWindow
    {
        #region Private Fields

        // UI State
        private Vector2 _assetsScrollPosition;
        private Vector2 _detailsScrollPosition;
        private float _containerItemHeight = 32f;
        private float _assetsScrollHeight = 300f;
        private bool _isResizingScroll = false;
        private Rect _resizeHandleRect;
        private const float RESIZE_HANDLE_HEIGHT = 10f;
        private bool _showNewProfileInput = false;
        private string _newProfileName = "";

        // Assets Management
        private List<UnityEngine.Object> _favoriteAssets = new List<UnityEngine.Object>();
        private ReorderableList _reorderableAssetsList;
        private Dictionary<int, AssetEditorState> _assetEditorStates = new Dictionary<int, AssetEditorState>();
        private static Stack<EditorLayoutState> _layoutStateStack = new Stack<EditorLayoutState>();

        // Profile Management
        private string _profilesDirectory;
        private List<string> _availableProfiles = new List<string>();
        private string _currentProfileName = "Default";
        private int _selectedProfileIndex = 0;

        #endregion

        #region Menu Items and Initialization

        [MenuItem("Tools/Favorite Assets")]
        public static void ShowWindow()
        {
            GetWindow<FavoriteAssets>("My Favorite Assets");
        }

        private void OnEnable()
        {
            InitializeProfilesDirectory();
            LoadCurrentProfileName();
            LoadAvailableProfiles();
            UpdateSelectedProfileIndex();
            LoadAssets();
            InitializeReorderableList();
        }

        private void InitializeProfilesDirectory()
        {
            _profilesDirectory = Path.Combine(Application.dataPath, "../Library/MyFavoriteAssetProfiles");

            if (!Directory.Exists(_profilesDirectory))
            {
                Directory.CreateDirectory(_profilesDirectory);
            }
        }

        private void OnDisable()
        {
            SaveAssets();
            SaveCurrentProfileName();
            DestroyAllEditors();
        }

        #endregion

        #region GUI Drawing

        private void OnGUI()
        {
            EditorGUILayout.Space();

            DrawProfilesGUI();
            DrawHelpBox();
            ProcessAssetDragAndDrop();
            DrawAssetsPanel();
            DrawResizeHandle();
            DrawDivider();
            DrawDetailsPanel();

            HandleEvents();
        }

        private void DrawHelpBox()
        {
            EditorGUILayout.HelpBox(
                "You can drag and drop any Unity asset to this window. Grab containers from their left side and drag to reorder them.",
                MessageType.Info);
        }

        private void DrawAssetsPanel()
        {
            EditorGUILayout.LabelField("My Favorite Assets", EditorStyles.boldLabel);

            // Add padding to prevent overlap with resize handle
            float scrollPadding = 6f;

            // Begin scroll view with dynamic height
            Rect scrollViewRect = EditorGUILayout.GetControlRect(false, _assetsScrollHeight - scrollPadding,
                GUILayout.ExpandWidth(true));
                
            _assetsScrollPosition = GUI.BeginScrollView(scrollViewRect, _assetsScrollPosition,
                new Rect(0, 0, scrollViewRect.width - 16,
                    // Calculate the content height
                    Mathf.Max(_containerItemHeight * _favoriteAssets.Count + 10f,
                        _assetsScrollHeight - scrollPadding)));

            if (_reorderableAssetsList != null && _favoriteAssets.Count > 0)
            {
                _reorderableAssetsList.DoList(new Rect(0, 0, scrollViewRect.width - 16,
                    Mathf.Max(_containerItemHeight * _favoriteAssets.Count + 10f,
                        _assetsScrollHeight - scrollPadding)));
            }
            else
            {
                EditorGUI.HelpBox(new Rect(0, 0, scrollViewRect.width - 16, 40),
                    "Drag and drop assets here to pin them.", MessageType.Info);
            }

            GUI.EndScrollView();

            // Add spacing between scroll view and resize handle
            EditorGUILayout.Space(scrollPadding);
        }

        private void DrawResizeHandle()
        {
            Rect scrollViewRect = GUILayoutUtility.GetLastRect();
            float scrollPadding = 6f;

            // Draw the resize handle
            _resizeHandleRect = new Rect(scrollViewRect.x, scrollViewRect.y + scrollViewRect.height + scrollPadding,
                scrollViewRect.width, RESIZE_HANDLE_HEIGHT);

            // Get appropriate colors based on theme
            Color handleColor = GetResizeHandleColor();
            
            EditorGUI.DrawRect(_resizeHandleRect, handleColor);

            // Draw grip lines for visual feedback
            DrawResizeHandleGripLines();

            // Change cursor when hovering over the resize handle
            if (_resizeHandleRect.Contains(Event.current.mousePosition) || _isResizingScroll)
            {
                EditorGUIUtility.AddCursorRect(_resizeHandleRect, MouseCursor.ResizeVertical);
            }
        }

        private void DrawResizeHandleGripLines()
        {
            Color gripColor = EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.0f)
                : new Color(0.3f, 0.3f, 0.3f, 1.0f);

            float centerY = _resizeHandleRect.y + _resizeHandleRect.height / 2;

            for (int i = 0; i < 5; i++)
            {
                float x = _resizeHandleRect.x + _resizeHandleRect.width / 2 - 12 + i * 6;
                EditorGUI.DrawRect(new Rect(x, centerY, 4, 1), gripColor);
            }
        }

        private Color GetResizeHandleColor()
        {
            Color baseColor = EditorGUIUtility.isProSkin
                ? new Color(0.35f, 0.35f, 0.35f, 1.0f)
                : new Color(0.7f, 0.7f, 0.7f, 1.0f);

            if (_isResizingScroll || _resizeHandleRect.Contains(Event.current.mousePosition))
            {
                // Highlight on hover or during resize
                return EditorGUIUtility.isProSkin
                    ? new Color(0.4f, 0.7f, 1.0f, 1.0f)
                    : new Color(0.2f, 0.5f, 0.9f, 1.0f);
            }

            return baseColor;
        }

        private void DrawDivider()
        {
            EditorGUILayout.Space(RESIZE_HANDLE_HEIGHT + 2);
            Rect dividerRect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(dividerRect,
                EditorGUIUtility.isProSkin ? new Color(0.1f, 0.1f, 0.1f) : new Color(0.5f, 0.5f, 0.5f, 0.5f));
            EditorGUILayout.Space(2);
        }

        private void DrawDetailsPanel()
        {
            // Only show title if any asset editor is active
            bool hasAnyExpandedEditor = _assetEditorStates.Any(pair => pair.Value.IsExpanded);
            if (hasAnyExpandedEditor)
            {
                EditorGUILayout.LabelField("Asset Details", EditorStyles.boldLabel);
                
                // Second panel: ScrollView for AssetEditors
                _detailsScrollPosition = EditorGUILayout.BeginScrollView(_detailsScrollPosition, GUILayout.ExpandHeight(true));

                DrawExpandedAssetDetails();

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawExpandedAssetDetails()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            SaveLayoutSettings();

            // Draw AssetEditors
            foreach (var pair in _assetEditorStates.Where(p => p.Value.IsExpanded))
            {
                EditorGUILayout.Space(5);

                // Get the asset from the editor state's target
                UnityEngine.Object targetAsset = pair.Value.Target;
                if (targetAsset != null)
                {
                    DrawAssetDetailsFoldout(pair.Key, pair.Value, targetAsset);
                }

                // Draw the inline editor
                DrawInlineEditor(pair.Value);
                EditorGUILayout.Space(5);
            }

            RestoreLayout();
            EditorGUILayout.EndVertical();
        }

        private void DrawAssetDetailsFoldout(int assetId, AssetEditorState editorState, UnityEngine.Object asset)
        {
            // Create foldout style
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            foldoutStyle.fontSize = 12;

            // Foldout header
            EditorGUILayout.BeginHorizontal();

            // Foldout control
            bool newFoldedOut = EditorGUILayout.Foldout(
                editorState.IsFoldedOut,
                asset.name,
                true,
                foldoutStyle
            );

            // Update if foldout state changed
            if (newFoldedOut != editorState.IsFoldedOut)
            {
                editorState.IsFoldedOut = newFoldedOut;
                // Immediately save state when folding/unfolding
                SaveAssets();
                Repaint();
            }

            EditorGUILayout.EndHorizontal();

            // Divider line
            Rect headerDivider = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(headerDivider,
                EditorGUIUtility.isProSkin
                    ? new Color(0.3f, 0.3f, 0.3f)
                    : new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        private void DrawInlineEditor(AssetEditorState state)
        {
            if (state == null || state.EditorInstance == null) return;

            try
            {
                // Only show content if foldout is open
                if (state.IsFoldedOut)
                {
                    // Show editor content in a box and slightly indent
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    // Draw header
                    if (state.ShouldDrawHeader)
                    {
                        EditorGUILayout.BeginFadeGroup(0.9999f);
                        GUILayout.Space(5.0f);
                        state.EditorInstance.DrawHeader();
                        GUILayout.Space(5f);
                        EditorGUILayout.EndFadeGroup();
                    }

                    // Draw the actual inspector GUI
                    if (state.ShouldDrawGUI)
                    {
                        EditorGUILayout.BeginVertical();
                        bool inspectorExpanded = InternalEditorUtility.GetIsInspectorExpanded(state.EditorInstance.target);
                        if (!state.ShouldDrawHeader)
                        {
                            InternalEditorUtility.SetIsInspectorExpanded(state.EditorInstance.target, true);
                        }

                        state.EditorInstance.OnInspectorGUI();

                        if (!state.ShouldDrawHeader)
                        {
                            InternalEditorUtility.SetIsInspectorExpanded(state.EditorInstance.target, inspectorExpanded);
                        }

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions safely to avoid breaking the editor
                if (ex is ExitGUIException)
                    throw;

                Debug.LogException(ex);
            }
        }

        private void DrawProfilesGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Current Profile:", GUILayout.Width(100));

            EditorGUI.BeginChangeCheck();
            _selectedProfileIndex =
                EditorGUILayout.Popup(_selectedProfileIndex, _availableProfiles.ToArray(), GUILayout.Width(150));
            if (EditorGUI.EndChangeCheck())
            {
                HandleProfileChange();
            }

            if (GUILayout.Button("Create Profile", GUILayout.Width(100)))
            {
                _showNewProfileInput = !_showNewProfileInput;
            }

            if (_availableProfiles.Count > 1 && GUILayout.Button("Delete Profile", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Delete Profile",
                        "Are you sure you want to delete the profile '" + _currentProfileName + "'?",
                        "Yes", "No"))
                {
                    DeleteCurrentProfile();
                }
            }

            EditorGUILayout.EndHorizontal();

            if (_showNewProfileInput)
            {
                DrawNewProfileInput();
            }
        }

        private void DrawNewProfileInput()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("New Profile Name:", GUILayout.Width(120));
            _newProfileName = EditorGUILayout.TextField(_newProfileName, GUILayout.Width(150));

            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_newProfileName) ||
                                         _availableProfiles.Contains(_newProfileName));
            if (GUILayout.Button("Create", GUILayout.Width(80)))
            {
                CreateNewProfile(_newProfileName);
                _showNewProfileInput = false;
                _newProfileName = "";
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                _showNewProfileInput = false;
                _newProfileName = "";
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Event Handling

        private void HandleEvents()
        {
            Event currentEvent = Event.current;
            
            // Handle resizing
            HandleResizing(currentEvent);
            
            // Handle drag and drop events
            if (currentEvent.type == EventType.MouseDrag || 
                currentEvent.type == EventType.DragUpdated ||
                currentEvent.type == EventType.DragPerform)
            {
                Repaint();
            }

            // Save on mouse up
            if (currentEvent.type == EventType.MouseUp)
            {
                SaveAssets();
            }
        }

        private void HandleResizing(Event e)
        {
            float scrollPadding = 6f; // Must match the padding in OnGUI

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (_resizeHandleRect.Contains(e.mousePosition))
                    {
                        _isResizingScroll = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (_isResizingScroll)
                    {
                        // Calculate new height based on mouse position
                        float minHeight = 100f; // Minimum height
                        float maxHeight = position.height - 200f; // Maximum height

                        // Adjust calculation to account for the padding
                        _assetsScrollHeight =
                            Mathf.Clamp(
                                e.mousePosition.y - _resizeHandleRect.y + RESIZE_HANDLE_HEIGHT + scrollPadding +
                                _assetsScrollHeight, minHeight, maxHeight);

                        Repaint();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (_isResizingScroll)
                    {
                        _isResizingScroll = false;
                        SaveAssets(); // Save the new height
                        e.Use();
                    }
                    break;
            }
        }

        private void ProcessAssetDragAndDrop()
        {
            Event evt = Event.current;
            Rect dropArea = new Rect(0, 0, position.width, position.height);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        evt.Use();
                    }
                    break;

                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.AcceptDrag();
                    HandleAssetDrop();
                    evt.Use();
                    break;
            }
        }

        private void HandleAssetDrop()
        {
            int duplicateCount = 0;

            foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
            {
                if (draggedObject != null)
                {
                    string draggedAssetPath = AssetDatabase.GetAssetPath(draggedObject);

                    bool alreadyExists = _favoriteAssets.Any(existingAsset => 
                        existingAsset != null && 
                        AssetDatabase.GetAssetPath(existingAsset) == draggedAssetPath);

                    if (alreadyExists)
                    {
                        duplicateCount++;
                    }
                    else
                    {
                        _favoriteAssets.Add(draggedObject);
                    }
                }
            }

            if (duplicateCount > 0)
            {
                string message = duplicateCount == 1
                    ? "One asset was not added because it already exists in the profile."
                    : $"{duplicateCount} assets were not added because they already exist in the profile.";

                EditorUtility.DisplayDialog("Duplicate Asset", message, "OK");
            }

            InitializeReorderableList();
            SaveAssets();
        }

        #endregion

        #region Asset List Management

        private void InitializeReorderableList()
        {
            _reorderableAssetsList = new ReorderableList(_favoriteAssets, typeof(UnityEngine.Object), true, false, false, false);

            _reorderableAssetsList.drawElementCallback = DrawAssetListItem;
            _reorderableAssetsList.elementHeightCallback = (int index) => _containerItemHeight;
            _reorderableAssetsList.headerHeight = 0;
            _reorderableAssetsList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                // Background handled in drawElementCallback for better control
            };
        }

        private void DrawAssetListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= _favoriteAssets.Count || _favoriteAssets[index] == null)
                return;

            Rect adjustedRect = new Rect(rect.x, rect.y + 1f, rect.width, rect.height - 2f);

            // Draw background
            DrawAssetItemBackground(adjustedRect, isActive, isFocused);

            // Draw asset field
            Rect fieldRect = new Rect(
                adjustedRect.x + 8,
                adjustedRect.y + (adjustedRect.height - EditorGUIUtility.singleLineHeight) / 2,
                adjustedRect.width / 3,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.BeginChangeCheck();
            UnityEngine.Object newObject = EditorGUI.ObjectField(
                fieldRect,
                GUIContent.none,
                _favoriteAssets[index],
                typeof(UnityEngine.Object),
                false
            );
            
            if (EditorGUI.EndChangeCheck())
            {
                HandleAssetReplacement(index, newObject);
            }

            // Draw buttons
            DrawAssetItemButtons(adjustedRect, index);
        }

        private void DrawAssetItemBackground(Rect rect, bool isActive, bool isFocused)
        {
            // Use theme-appropriate background color
            Color bgColor = EditorGUIUtility.isProSkin
                ? new Color(0.22f, 0.22f, 0.22f, 1.0f)
                : new Color(0.76f, 0.76f, 0.76f, 1.0f);

            EditorGUI.DrawRect(rect, bgColor);

            // Create a subtle border effect
            if (isActive || isFocused)
            {
                // Active/focused border color
                Color borderColor = EditorGUIUtility.isProSkin
                    ? new Color(0.32f, 0.62f, 0.95f, 1.0f)
                    : new Color(0.22f, 0.52f, 0.85f, 1.0f);

                Rect borderRect = new Rect(rect.x, rect.y, rect.width, 1);
                EditorGUI.DrawRect(borderRect, borderColor);

                borderRect.y = rect.y + rect.height - 1;
                EditorGUI.DrawRect(borderRect, borderColor);

                borderRect = new Rect(rect.x, rect.y, 1, rect.height);
                EditorGUI.DrawRect(borderRect, borderColor);

                borderRect.x = rect.x + rect.width - 1;
                EditorGUI.DrawRect(borderRect, borderColor);
            }
        }

        private void DrawAssetItemButtons(Rect adjustedRect, int index)
        {
            UnityEngine.Object asset = _favoriteAssets[index];
            if (asset == null) return;

            // Button layout
            float buttonSize = EditorGUIUtility.singleLineHeight * 1.2f;
            float startX = adjustedRect.x + adjustedRect.width / 3 + 20;
            float buttonY = adjustedRect.y + (adjustedRect.height - buttonSize) / 2;

            // Calculate button rectangles
            Rect openButtonRect = new Rect(startX, buttonY, 64, buttonSize);
            Rect inspectButtonRect = new Rect(startX + 70, buttonY, 64, buttonSize);
            Rect deleteButtonRect = new Rect(startX + 140, buttonY, 24, buttonSize);
            Rect moreInfoButtonRect = new Rect(startX + 170, buttonY, 24, buttonSize);

            // Open button
            GUIContent openButtonContent = new GUIContent(" Open");
            openButtonContent.tooltip = "Click to open the asset";

            if (GUI.Button(openButtonRect, openButtonContent))
            {
                Event e = Event.current;
                if (e.type == EventType.Used && openButtonRect.Contains(e.mousePosition))
                {
                    OpenAsset(asset);
                }
            }

            // Inspect button
            GUIContent inspectButtonContent = new GUIContent("Inspect");
            inspectButtonContent.tooltip = "Click to inspect the asset in Inspector";

            if (GUI.Button(inspectButtonRect, inspectButtonContent))
            {
                Event e = Event.current;
                if (e.type == EventType.Used && inspectButtonRect.Contains(e.mousePosition))
                {
                    Selection.activeObject = asset;
                }
            }

            // Delete button
            GUIContent deleteButtonContent = new GUIContent();
            deleteButtonContent.image = EditorGUIUtility.IconContent("Toolbar Minus").image;
            deleteButtonContent.tooltip = "Click to remove this asset";

            if (GUI.Button(deleteButtonRect, deleteButtonContent))
            {
                Event e = Event.current;
                if (e.type == EventType.Used && deleteButtonRect.Contains(e.mousePosition))
                {
                    RemoveAsset(index);
                    e.Use();
                }
            }

            // More info button
            DrawMoreInfoButton(moreInfoButtonRect, asset);
        }

        private void DrawMoreInfoButton(Rect buttonRect, UnityEngine.Object asset)
        {
            if (asset == null) return;

            GUIContent buttonContent = new GUIContent();
            int instanceID = asset.GetInstanceID();
            bool hasInlineEditor = _assetEditorStates.ContainsKey(instanceID);

            // Use different icon when expanded
            if (hasInlineEditor && _assetEditorStates[instanceID].IsExpanded)
            {
                buttonContent.image = EditorGUIUtility.IconContent("d_animationvisibilitytoggleoff@2x").image;
            }
            else
            {
                buttonContent.image = EditorGUIUtility.IconContent("d_animationvisibilitytoggleon@2x").image;
            }

            buttonContent.tooltip = "Click for more info";

            if (GUI.Button(buttonRect, buttonContent))
            {
                Event e = Event.current;
                if (e.type == EventType.Used && buttonRect.Contains(e.mousePosition))
                {
                    ToggleAssetDetails(asset, instanceID, hasInlineEditor);
                }
            }
        }

        private void ToggleAssetDetails(UnityEngine.Object asset, int instanceID, bool hasInlineEditor)
        {
            if (!hasInlineEditor)
            {
                // Create inline editor state
                AssetEditorState state = new AssetEditorState
                {
                    IsExpanded = true,
                    Target = asset,
                    IsFoldedOut = true // Default is expanded
                };
                
                _assetEditorStates[instanceID] = state;
                UpdateEditors(state);
            }
            else
            {
                // Toggle expanded state
                _assetEditorStates[instanceID].IsExpanded = !_assetEditorStates[instanceID].IsExpanded;
                if (!_assetEditorStates[instanceID].IsExpanded)
                {
                    DestroyEditor(_assetEditorStates[instanceID]);
                }
                else if (_assetEditorStates[instanceID].EditorInstance == null)
                {
                    UpdateEditors(_assetEditorStates[instanceID]);
                }
            }

            // Force repaint to update UI
            Repaint();

            // Save state immediately when toggling
            SaveAssets();
        }

        private void HandleAssetReplacement(int index, UnityEngine.Object newObject)
        {
            // If changed, remove old asset's editor state if it exists
            if (_favoriteAssets[index] != null)
            {
                int oldInstanceID = _favoriteAssets[index].GetInstanceID();
                if (_assetEditorStates.ContainsKey(oldInstanceID))
                {
                    DestroyEditor(_assetEditorStates[oldInstanceID]);
                    _assetEditorStates.Remove(oldInstanceID);
                }
            }

            _favoriteAssets[index] = newObject;
            SaveAssets();
        }

        private void RemoveAsset(int index)
        {
            UnityEngine.Object asset = _favoriteAssets[index];
            
            // Remove associated editor state
            if (asset != null)
            {
                int instanceID = asset.GetInstanceID();
                if (_assetEditorStates.ContainsKey(instanceID))
                {
                    DestroyEditor(_assetEditorStates[instanceID]);
                    _assetEditorStates.Remove(instanceID);
                }
            }

            _favoriteAssets.RemoveAt(index);
            SaveAssets();
        }

        private void OpenAsset(UnityEngine.Object asset)
        {
            if (asset == null)
                return;

            if (asset is GameObject prefab)
            {
                AssetDatabase.OpenAsset(prefab);
            }
            else if (asset is ScriptableObject || 
                     asset is Texture || 
                     asset is Texture2D || 
                     asset is Sprite ||
                     asset is Material)
            {
                Selection.activeObject = asset;
            }
            else
            {
                AssetDatabase.OpenAsset(asset);
            }
        }

        #endregion

        #region Editor Management

        private void UpdateEditors(AssetEditorState state)
        {
            if (state == null || state.Target == null) return;

            bool isGameObject = state.Target is GameObject;

            // Setup editor properties based on asset type
            state.ShouldDrawHeader = true;
            state.ShouldDrawGUI = !isGameObject;
            state.ShouldDrawPreview = true; // Always allow preview

            // Create the main editor
            state.EditorInstance = Editor.CreateEditor(state.Target);

            // Create preview editor if needed
            Component targetAsComponent = state.Target as Component;
            if (targetAsComponent != null)
            {
                state.PreviewEditorInstance = Editor.CreateEditor(targetAsComponent.gameObject);
            }
            else
            {
                state.PreviewEditorInstance = state.EditorInstance;
            }
        }

        private void DestroyAllEditors()
        {
            foreach (var state in _assetEditorStates.Values)
            {
                DestroyEditor(state);
            }

            _assetEditorStates.Clear();
        }

        private void DestroyEditor(AssetEditorState state)
        {
            if (state.EditorInstance != null)
            {
                UnityEngine.Object.DestroyImmediate(state.EditorInstance);
                state.EditorInstance = null;
            }

            if (state.PreviewEditorInstance != null && state.PreviewEditorInstance != state.EditorInstance)
            {
                UnityEngine.Object.DestroyImmediate(state.PreviewEditorInstance);
                state.PreviewEditorInstance = null;
            }
        }

        #endregion

        #region Profile Management

        private void HandleProfileChange()
        {
            SaveAssets();
            _currentProfileName = _availableProfiles[_selectedProfileIndex];
            SaveCurrentProfileName();

            // Clear existing editor states before loading the new profile
            DestroyAllEditors();

            // Load the assets and their states from the new profile
            LoadAssets();

            // Recreate the list and update the UI
            InitializeReorderableList();
        }

        private void LoadAvailableProfiles()
        {
            _availableProfiles.Clear();

            if (Directory.Exists(_profilesDirectory))
            {
                string[] profileFiles = Directory.GetFiles(_profilesDirectory, "*.json");
                foreach (string profilePath in profileFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(profilePath);
                    if (fileName != "CurrentProfile")
                    {
                        _availableProfiles.Add(fileName);
                    }
                }
            }

            if (_availableProfiles.Count == 0)
            {
                _availableProfiles.Add("Default");
            }
        }

        private void UpdateSelectedProfileIndex()
        {
            _selectedProfileIndex = _availableProfiles.IndexOf(_currentProfileName);
            if (_selectedProfileIndex < 0)
            {
                _selectedProfileIndex = 0;
                _currentProfileName = _availableProfiles[0];
            }
        }

        private void LoadCurrentProfileName()
        {
            string currentProfilePath = Path.Combine(_profilesDirectory, "CurrentProfile.json");

            if (File.Exists(currentProfilePath))
            {
                string jsonData = File.ReadAllText(currentProfilePath);
                ProfileNameData profileNameData = JsonUtility.FromJson<ProfileNameData>(jsonData);

                if (profileNameData != null && !string.IsNullOrEmpty(profileNameData.CurrentProfileName))
                {
                    _currentProfileName = profileNameData.CurrentProfileName;
                }
            }
            else
            {
                _currentProfileName = "Default";
            }
        }

        private void SaveCurrentProfileName()
        {
            string currentProfilePath = Path.Combine(_profilesDirectory, "CurrentProfile.json");

            ProfileNameData profileNameData = new ProfileNameData();
            profileNameData.CurrentProfileName = _currentProfileName;

            string jsonData = JsonUtility.ToJson(profileNameData, true);
            File.WriteAllText(currentProfilePath, jsonData);
        }

        private void CreateNewProfile(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName) || _availableProfiles.Contains(profileName))
                return;

            SaveAssets();

            _currentProfileName = profileName;
            _favoriteAssets.Clear();
            _assetEditorStates.Clear(); // Clear editor states for the new profile
            SaveAssets();

            _availableProfiles.Add(profileName);
            UpdateSelectedProfileIndex();

            SaveCurrentProfileName();
        }

        private void DeleteCurrentProfile()
        {
            if (_availableProfiles.Count <= 1)
                return;

            string profilePath = Path.Combine(_profilesDirectory, _currentProfileName + ".json");

            if (File.Exists(profilePath))
            {
                File.Delete(profilePath);
            }

            _availableProfiles.Remove(_currentProfileName);

            _currentProfileName = _availableProfiles[0];
            UpdateSelectedProfileIndex();

            LoadAssets();
            InitializeReorderableList();
            SaveCurrentProfileName();
        }

        #endregion

        #region Data Serialization

        private void SaveAssets()
        {
            AssetProfileData saveData = new AssetProfileData();
            saveData.AssetPaths = new List<string>();
            saveData.ExpandedAssetPaths = new List<string>();
            saveData.FoldedOutStates = new List<bool>();
            // Add the scroll view height to the save data
            saveData.AssetsScrollHeight = _assetsScrollHeight;

            foreach (UnityEngine.Object asset in _favoriteAssets)
            {
                if (asset != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(asset);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        saveData.AssetPaths.Add(assetPath);

                        // Check if this asset has expanded editor state and save it
                        int instanceID = asset.GetInstanceID();
                        bool isExpanded = _assetEditorStates.ContainsKey(instanceID) &&
                                          _assetEditorStates[instanceID].IsExpanded;
                        bool isFoldedOut = isExpanded && _assetEditorStates[instanceID].IsFoldedOut;

                        if (isExpanded)
                        {
                            saveData.ExpandedAssetPaths.Add(assetPath);
                            saveData.FoldedOutStates.Add(isFoldedOut);
                        }
                    }
                }
            }

            string profilePath = Path.Combine(_profilesDirectory, _currentProfileName + ".json");
            string jsonData = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(profilePath, jsonData);
        }

        private void LoadAssets()
        {
            _favoriteAssets.Clear();
            _assetEditorStates.Clear();

            string profilePath = Path.Combine(_profilesDirectory, _currentProfileName + ".json");

            if (File.Exists(profilePath))
            {
                string jsonData = File.ReadAllText(profilePath);
                AssetProfileData loadedData = JsonUtility.FromJson<AssetProfileData>(jsonData);

                if (loadedData != null)
                {
                    // Load the scroll view height if available
                    if (loadedData.AssetsScrollHeight > 0)
                    {
                        _assetsScrollHeight = loadedData.AssetsScrollHeight;
                    }

                    if (loadedData.AssetPaths != null)
                    {
                        // First load all assets
                        foreach (string assetPath in loadedData.AssetPaths)
                        {
                            UnityEngine.Object loadedAsset =
                                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                            if (loadedAsset != null)
                            {
                                _favoriteAssets.Add(loadedAsset);
                            }
                        }

                        // Then restore expanded states for those that need it
                        if (loadedData.ExpandedAssetPaths != null)
                        {
                            RestoreAssetEditorStates(loadedData);
                        }
                    }
                }
            }
        }

        private void RestoreAssetEditorStates(AssetProfileData loadedData)
        {
            for (int i = 0; i < loadedData.ExpandedAssetPaths.Count; i++)
            {
                string expandedAssetPath = loadedData.ExpandedAssetPaths[i];
                UnityEngine.Object expandedAsset =
                    AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(expandedAssetPath);

                if (expandedAsset != null)
                {
                    int instanceID = expandedAsset.GetInstanceID();

                    // Create and setup the inline editor state
                    AssetEditorState state = new AssetEditorState
                    {
                        IsExpanded = true,
                        Target = expandedAsset
                    };

                    // Set folded out state if we have that information
                    if (loadedData.FoldedOutStates != null && i < loadedData.FoldedOutStates.Count)
                    {
                        state.IsFoldedOut = loadedData.FoldedOutStates[i];
                    }
                    else
                    {
                        state.IsFoldedOut = true; // Default to expanded
                    }

                    _assetEditorStates[instanceID] = state;
                    UpdateEditors(state);
                }
            }
        }

        #endregion

        #region Layout Helper Methods

        private static void SaveLayoutSettings()
        {
            _layoutStateStack.Push(new EditorLayoutState()
            {
                Skin = GUI.skin,
                Color = GUI.color,
                ContentColor = GUI.contentColor,
                BackgroundColor = GUI.backgroundColor,
                Enabled = GUI.enabled,
                IndentLevel = EditorGUI.indentLevel,
                FieldWidth = EditorGUIUtility.fieldWidth,
                LabelWidth = EditorGUIUtility.labelWidth,
                HierarchyMode = EditorGUIUtility.hierarchyMode,
                WideMode = EditorGUIUtility.wideMode
            });
        }

        private static void RestoreLayout()
        {
            EditorLayoutState layoutSettings = _layoutStateStack.Pop();
            GUI.skin = layoutSettings.Skin;
            GUI.color = layoutSettings.Color;
            GUI.contentColor = layoutSettings.ContentColor;
            GUI.backgroundColor = layoutSettings.BackgroundColor;
            GUI.enabled = layoutSettings.Enabled;
            EditorGUI.indentLevel = layoutSettings.IndentLevel;
            EditorGUIUtility.fieldWidth = layoutSettings.FieldWidth;
            EditorGUIUtility.labelWidth = layoutSettings.LabelWidth;
            EditorGUIUtility.hierarchyMode = layoutSettings.HierarchyMode;
            EditorGUIUtility.wideMode = layoutSettings.WideMode;
        }

        #endregion
    }
    
}