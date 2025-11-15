using CustomNPCTest.NPCs;
using MelonLoader;
using S1API.Entities;
using S1API.Entities.NPCs.Northtown;
using S1API.Input;
using S1API.Internal.Abstraction;
using S1API.Internal.Utils;
using S1API.PhoneApp;
using S1API.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoreNPCs.Apps
{
    public class FannsosContacts : PhoneApp
    {
        protected override string AppName => "FannsosContacts";
        protected override string AppTitle => "Fannso's Contacts";
        protected override string IconLabel => "Contacts";
        protected override string IconFileName => string.Empty;

        private GameObject _mapParentGO;
        private GameObject _mapContentGO;
        private RectTransform _mapContentRT;
        private GameObject _linesFolderGO;

        private readonly string[] _regions = { "Northtown", "Westville", "Downtown", "Docks", "Suburbia", "Uptown" };
        private string _currentRegion = "Northtown";

        private bool _dragging;
        private Vector2 _lastPointerPos;
        private float _zoom = 1f;

        private const float Y_FLIP = -1f;

        private RawImage _tiledBg;

        private const float TILE_PX = 64f;
        private Texture2D _tiledTex;
        private float _tilePx = TILE_PX; // effective tile size (src + padding)


        private readonly Dictionary<string, Image> _portraits = new();

        private readonly Dictionary<string, Vector2> _npcPositions = new()
        {
            {"KyleCooley", new Vector2(0,150)},
            {"AustinSteiner", new Vector2(100,50)},
            {"JasonReed", new Vector2(0,50)},
            {"KathyHenderson", new Vector2(200,50)},
            {"DonnaMartin", new Vector2(300,0)},
            {"PeggyMyers", new Vector2(400,50)},
            {"BethPenn", new Vector2(500,100)}, // updated
            {"Ming", new Vector2(500,250)},     // updated
            {"LudwigMeyer", new Vector2(400,350)},
            {"PeterFile", new Vector2(200,350)},
            {"SamThompson", new Vector2(200,250)},
            {"JessiWaters", new Vector2(100,250)},
            {"AlbertHoover", new Vector2(100,150)},
            {"MickLubbin", new Vector2(200,150)},
            {"GeraldinePoon", new Vector2(300,100)},
            {"BenjiColeman", new Vector2(400,200)},
            {"ChloeBowers", new Vector2(300,250)},
        };

        private readonly (string, string[])[] _connections =
        {
            ("KyleCooley", new[]{"AlbertHoover","AustinSteiner","JessiWaters","JasonReed"}),
            ("AlbertHoover", new[]{"JessiWaters","SamThompson","AustinSteiner","KyleCooley"}),
            ("AustinSteiner", new[]{"KathyHenderson","JasonReed"}),
            ("KathyHenderson", new[]{"DonnaMartin","GeraldinePoon"}),
            ("PeggyMyers", new[]{"GeraldinePoon","DonnaMartin","BethPenn"}),
            ("BethPenn", new[]{"BenjiColeman","PeggyMyers","Ming"}),
            ("Ming", new[]{"BethPenn","LudwigMeyer"}),
            ("LudwigMeyer", new[]{"BenjiColeman","PeterFile"}),
            ("ChloeBowers", new[]{"SamThompson","BenjiColeman"}),
            ("JessiWaters", new[]{"SamThompson"}),
            ("MickLubbin", new[]{"GeraldinePoon","SamThompson"}),
            ("SamThompson", new[]{"PeterFile"})
        };

        private Sprite LoadEmbeddedSprite(string resourceName)
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    MelonLogger.Warning($"[FannsosContacts] Embedded resource not found: {resourceName}");
                    return null;
                }

                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return ImageUtils.LoadImageRaw(data);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[FannsosContacts] Failed to load embedded sprite '{resourceName}': {ex.Message}");
                return null;
            }
        }

        private void CreateTiledBackground(Transform parent)
        {
            var bgGO = new GameObject("TiledBG");
            bgGO.transform.SetParent(parent, false);

            var rt = bgGO.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            _tiledBg = bgGO.AddComponent<RawImage>();
            _tiledBg.raycastTarget = false;

            // Load the embedded tile and build a padded, repeatable texture
            var tileSprite = LoadEmbeddedSprite("MoreNPCs.Resources.tile.png");
            if (tileSprite != null && tileSprite.texture != null)
            {
                var src = tileSprite.texture;
                var lightGray = new Color(0.83f, 0.83f, 0.83f, 1f); // tile tint
                var gapGray = new Color(0.20f, 0.20f, 0.20f, 1f); // lines between tiles

                int pad = 5;
                int w = src.width + pad * 2;
                int h = src.height + pad * 2;

                _tiledTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                _tiledTex.wrapMode = TextureWrapMode.Repeat;   // <-- important
                _tiledTex.filterMode = FilterMode.Bilinear;

                // fill with the "between tiles" dark gray
                var fill = new Color[w * h];
                for (int i = 0; i < fill.Length; i++) fill[i] = gapGray;
                _tiledTex.SetPixels(fill);

                // copy + tint center tile
                var srcPixels = src.GetPixels();
                for (int y = 0; y < src.height; y++)
                    for (int x = 0; x < src.width; x++)
                    {
                        var c = srcPixels[y * src.width + x];
                        c = new Color(c.r * lightGray.r, c.g * lightGray.g, c.b * lightGray.b, c.a);
                        _tiledTex.SetPixel(x + pad, y + pad, c);
                    }

                _tiledTex.Apply();
                _tiledBg.texture = _tiledTex;

                // each “unit” repeat = tile + 5px padding on all sides
                _tilePx = w;
            }
            else
            {
                // fallback to your existing generated grid if tile.png isn't there
                _tiledBg.texture = GenerateGridTexture(64, 64,
                    new Color(0.125f, 0.125f, 0.125f, 1f),
                    new Color(0.16f, 0.16f, 0.16f, 1f),
                    new Color(0.20f, 0.20f, 0.20f, 1f));
            }

            // render behind map content
            bgGO.transform.SetAsFirstSibling();

            UpdateTiledBG(); // initial UVs
        }

        private void UpdateTiledBG()
        {
            if (_tiledBg == null || _mapParentGO == null || _mapContentRT == null) return;

            var parentRect = _mapParentGO.GetComponent<RectTransform>().rect;

            float scale = Mathf.Max(0.0001f, _zoom);
            float unit = Mathf.Max(1f, _tilePx); // fall back safely

            // how many repeats should be visible on screen
            float tilesX = Mathf.Max(2f, parentRect.width / (unit * scale));
            float tilesY = Mathf.Max(2f, parentRect.height / (unit * scale));

            // pan offset in tile units, wrapped so it doesn't grow unbounded
            Vector2 p = _mapContentRT.anchoredPosition;
            float offX = (p.x / (unit * scale)) % 1f;
            float offY = (-p.y / (unit * scale)) % 1f;
            if (offX < 0) offX += 1f;
            if (offY < 0) offY += 1f;

            _tiledBg.uvRect = new Rect(offX, offY, tilesX, tilesY);
        }

        private Texture2D GenerateGridTexture(int w, int h, Color bg, Color cell, Color grid)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;

            // simple checker with a subtle grid cross
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bool checker = ((x / 8) + (y / 8)) % 2 == 0;
                    Color c = checker ? cell : bg;

                    // thin grid lines every 16 px
                    if (x % 16 == 0 || y % 16 == 0) c = Color.Lerp(c, grid, 0.65f);
                    tex.SetPixel(x, y, c);
                }

            tex.Apply();
            return tex;
        }

        protected override void OnCreated()
        {
            base.OnCreated();

            try
            {
                // Load the embedded circular mask/icon sprite
                var iconSprite = LoadEmbeddedSprite("MoreNPCs.Resources.circle_mask.png");
                if (iconSprite != null)
                {
                    SetIconSprite(iconSprite);
                    MelonLogger.Msg("[FannsosContacts] Embedded app icon loaded successfully!");
                }
                else
                {
                    MelonLogger.Warning("[FannsosContacts] Embedded circle_mask.png not found.");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[FannsosContacts] Failed to load app icon: {ex.Message}");
            }
        }

        protected override void OnCreatedUI(GameObject container)
        {
            try
            {
                var bg = UIFactory.Panel("MainBG", container.transform, new Color(0.12f, 0.12f, 0.12f, 1f), fullAnchor: true);

                // Top bar
                var topBar = UIFactory.TopBar("TopBar", bg.transform, "Contacts", 0.9f, 45, 45, 0, 29);
                foreach (var region in _regions)
                {
                    var (btnGO, btn, lbl) = UIFactory.RoundedButtonWithLabel(
                        region + "Btn",
                        region,
                        topBar.transform,
                        new Color(0, 0, 0, 0),
                        110,
                        36,
                        16,
                        region == _currentRegion ? Color.white : Color.gray
                    );
                    string captured = region;
                    ButtonUtils.ClearListeners(btn);
                    ButtonUtils.AddListener(btn, () => SelectRegion(captured, topBar.transform));
                }

                _mapParentGO = UIFactory.Panel("MapArea", bg.transform, new Color(0.10f, 0.10f, 0.10f, 1f),
                    new Vector2(0f, 0f), new Vector2(1f, 0.9f));
                // make the phone screen clip the map visuals
                var mask = _mapParentGO.AddComponent<Mask>();
                CreateTiledBackground(_mapParentGO.transform);
                mask.showMaskGraphic = false; // hides the gray mask itself if you don’t want to see it


                _mapContentGO = UIFactory.Panel("MapContent", _mapParentGO.transform, new Color(0, 0, 0, 0));
                _mapContentRT = _mapContentGO.GetComponent<RectTransform>();
                _mapContentRT.anchorMin = _mapContentRT.anchorMax = new Vector2(0.5f, 0.5f);
                _mapContentRT.pivot = new Vector2(0.5f, 0.5f);
                _mapContentRT.anchoredPosition = Vector2.zero;
                _mapContentRT.sizeDelta = Vector2.zero;
                _mapContentRT.localScale = Vector3.one;

                _linesFolderGO = new GameObject("Lines");
                _linesFolderGO.transform.SetParent(_mapContentGO.transform, false);

                DrawConnections();
                DrawNPCs();
                AddPanZoomEvents(_mapParentGO);

                // load icons after short delay so NPCs are ready
                MelonCoroutines.Start(FillNpcIconsWhenReady());
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"FannsosContacts.OnCreatedUI failed: {ex}");
            }
        }

        private void SelectRegion(string region, Transform topBar)
        {
            _currentRegion = region;
            foreach (Transform child in topBar)
            {
                var txt = child.GetComponentInChildren<Text>();
                if (txt != null)
                    txt.color = (txt.text == region) ? Color.white : Color.gray;
            }
        }

        private void DrawNPCs()
        {
            if (_mapContentGO == null) return;

            foreach (var kvp in _npcPositions)
            {
                var name = kvp.Key;
                var pos = kvp.Value;

                try
                {
                    // Root container (transparent, square base)
                    var npcGO = UIFactory.Panel(name, _mapContentGO.transform, new Color(0, 0, 0, 0));
                    if (npcGO == null) continue;

                    var rt = npcGO.GetComponent<RectTransform>();
                    if (rt == null) continue;

                    rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.sizeDelta = new Vector2(60, 60); // square, fits circle perfectly
                    rt.anchoredPosition = new Vector2(pos.x, pos.y * Y_FLIP);

                    // === MASK WRAPPER ===
                    var maskGO = new GameObject("Mask");
                    maskGO.transform.SetParent(npcGO.transform, false);
                    var maskRT = maskGO.AddComponent<RectTransform>();
                    maskRT.anchorMin = new Vector2(0, 0);
                    maskRT.anchorMax = new Vector2(1, 1);
                    // shrink inward by 5% on each side (total 10% smaller = 90%)
                    maskRT.offsetMin = new Vector2(
                        rt.sizeDelta.x * 0.09f,
                        rt.sizeDelta.y * 0.09f
                    );
                    maskRT.offsetMax = new Vector2(
                        -rt.sizeDelta.x * 0.09f,
                        -rt.sizeDelta.y * 0.09f
                    );

                    var maskImg = maskGO.AddComponent<Image>();
                    maskImg.sprite = LoadEmbeddedSprite("MoreNPCs.Resources.circle_mask.png");
                    maskImg.type = Image.Type.Simple;
                    maskImg.color = new Color(1f, 1f, 1f, 0.01f); // low alpha so mask works
                    var mask = maskGO.AddComponent<Mask>();
                    mask.showMaskGraphic = false; // don’t show the mask image itself

                    // === PORTRAIT IMAGE ===
                    var portraitGO = new GameObject("Portrait");
                    portraitGO.transform.SetParent(maskGO.transform, false);
                    var portraitRT = portraitGO.AddComponent<RectTransform>();
                    portraitRT.anchorMin = new Vector2(0, 0);
                    portraitRT.anchorMax = new Vector2(1, 1);
                    portraitRT.offsetMin = Vector2.zero;
                    portraitRT.offsetMax = Vector2.zero;

                    var img = portraitGO.AddComponent<Image>();
                    img.type = Image.Type.Simple;
                    img.preserveAspect = true;
                    img.enabled = false; // will enable once icon loads
                    _portraits[name] = img;

                    // --- Border ---
                    var borderGO = new GameObject("Border");
                    borderGO.transform.SetParent(npcGO.transform, false);
                    var borderRT = borderGO.AddComponent<RectTransform>();
                    borderRT.anchorMin = new Vector2(0, 0);
                    borderRT.anchorMax = new Vector2(1, 1);
                    borderRT.offsetMin = Vector2.zero;
                    borderRT.offsetMax = Vector2.zero;

                    var borderImg = borderGO.AddComponent<Image>();
                    borderImg.sprite = LoadEmbeddedSprite("MoreNPCs.Resources.relation_circle.png");
                    borderImg.type = Image.Type.Simple;
                    borderImg.color = Color.white; // or tint if you want: new Color(0.8f, 0.8f, 0.8f, 1f)
                    borderGO.transform.SetAsFirstSibling();
                }
                catch (System.Exception ex)
                {
                    MelonLogger.Error($"DrawNPCs failed for {name}: {ex.Message}");
                }
            }
        }


        private void DrawConnections()
        {
            if (_mapContentGO == null) return;
            var lineSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd");
            var drawn = new HashSet<string>();

            foreach (var (from, targets) in _connections)
            {
                if (!_npcPositions.TryGetValue(from, out var fromPos))
                    continue;

                foreach (var to in targets)
                {
                    if (!_npcPositions.TryGetValue(to, out var toPos))
                        continue;

                    string a = from, b = to;
                    if (string.Compare(a, b) > 0) { var t = a; a = b; b = t; }
                    string key = $"{a}__{b}";
                    if (drawn.Contains(key)) continue;
                    drawn.Add(key);

                    var lineGO = new GameObject($"Line_{a}_{b}");
                    lineGO.transform.SetParent(_linesFolderGO.transform, false);

                    var img = lineGO.AddComponent<Image>();
                    img.sprite = lineSprite;
                    img.type = Image.Type.Sliced;
                    img.color = new Color(0.1415f, 0.1415f, 0.1415f, 1f);

                    var rt = img.GetComponent<RectTransform>();
                    rt.anchorMin = rt.anchorMax = new Vector2(0, 0);

                    Vector2 flippedFrom = new Vector2(fromPos.x, fromPos.y * Y_FLIP);
                    Vector2 flippedTo = new Vector2(toPos.x, toPos.y * Y_FLIP);
                    Vector2 dir = flippedTo - flippedFrom;
                    float length = dir.magnitude;

                    rt.sizeDelta = new Vector2(length, 4f);
                    rt.pivot = new Vector2(0, 0.5f);
                    rt.anchoredPosition = flippedFrom;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    rt.localRotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }

        private void AddPanZoomEvents(GameObject target)
        {
            var trigger = target.GetComponent<EventTrigger>();
            if (trigger == null) trigger = target.AddComponent<EventTrigger>();

            EventHelper.AddEventTrigger(trigger, EventTriggerType.PointerDown, (BaseEventData bed) =>
            {
                if (bed is PointerEventData ped)
                {
                    _dragging = true;
                    _lastPointerPos = ped.position;
                }
            });

            EventHelper.AddEventTrigger(trigger, EventTriggerType.Drag, (BaseEventData bed) =>
            {
                if (!_dragging) return;
                if (bed is PointerEventData ped)
                {
                    Vector2 current = ped.position;
                    Vector2 delta = current - _lastPointerPos;
                    _lastPointerPos = current;
                    _mapContentRT.anchoredPosition += delta;
                }
            });

            EventHelper.AddEventTrigger(trigger, EventTriggerType.PointerUp, () => { _dragging = false; });

            EventHelper.AddEventTrigger(trigger, EventTriggerType.Scroll, (BaseEventData bed) =>
            {
                if (bed is PointerEventData ped)
                {
                    float scroll = ped.scrollDelta.y;
                    if (Mathf.Abs(scroll) > 0.001f)
                    {
                        _zoom = Mathf.Clamp(_zoom + Mathf.Sign(scroll) * 0.1f, 0.5f, 2.5f);
                        _mapContentRT.localScale = Vector3.one * _zoom;
                    }
                }
            });
        }

        private System.Collections.IEnumerator FillNpcIconsWhenReady()
        {
            yield return new WaitForSeconds(1f); // delay so NPCs exist

            foreach (var kvp in _portraits)
            {
                var name = kvp.Key;
                var img = kvp.Value;
                if (img == null) continue;

                Sprite icon = null;
                try
                {
                    if (name == "KyleCooley") icon = NPC.Get<KyleCooley>()?.Icon;
                    else if (name == "AustinSteiner") icon = NPC.Get<AustinSteiner>()?.Icon;
                    else if (name == "JasonReed") icon = NPC.Get<JasonReed>()?.Icon;
                    else if (name == "KathyHenderson") icon = NPC.Get<KathyHenderson>()?.Icon;
                    else if (name == "DonnaMartin") icon = NPC.Get<DonnaMartin>()?.Icon;
                    else if (name == "PeggyMyers") icon = NPC.Get<PeggyMyers>()?.Icon;
                    else if (name == "BethPenn") icon = NPC.Get<BethPenn>()?.Icon;
                    else if (name == "Ming") icon = NPC.Get<Ming>()?.Icon;
                    else if (name == "LudwigMeyer") icon = NPC.Get<LudwigMeyer>()?.Icon;
                    else if (name == "PeterFile") icon = NPC.Get<PeterFile>()?.Icon;
                    else if (name == "SamThompson") icon = NPC.Get<SamThompson>()?.Icon;
                    else if (name == "JessiWaters") icon = NPC.Get<JessiWaters>()?.Icon;
                    else if (name == "AlbertHoover") icon = NPC.Get<AlbertHoover>()?.Icon;
                    else if (name == "MickLubbin") icon = NPC.Get<MickLubbin>()?.Icon;
                    else if (name == "GeraldinePoon") icon = NPC.Get<GeraldinePoon>()?.Icon;
                    else if (name == "BenjiColeman") icon = NPC.Get<BenjiColeman>()?.Icon;
                    else if (name == "ChloeBowers") icon = NPC.Get<ChloeBowers>()?.Icon;
                }
                catch { }

                if (icon != null)
                {
                    img.sprite = icon;
                    img.enabled = true;
                }
            }
        }

        protected override void OnPhoneClosed()
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
