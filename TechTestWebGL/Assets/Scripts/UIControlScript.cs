using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace TechnicalTest
{
    public class UIControlScript : MonoBehaviour
    {
        public GameObject UIPanel;
        public GameObject UIOverallPartList;
        public GameObject UIMagazinePartList;
        public GameObject UIReceiverPartList;
        public GameObject UISightPartList;
        public GameObject UIStockPartList;
        public GameObject UIChildObjectPrefab;
        public GameObject UIScrollPanel;
        //public GameObject UITempTrial;

        private Vector3 holdHitPos;
        private Vector3 UIPartListVecDistance;
        private Vector3 UIPartChildListVecXDistance;
        private Vector3 arrowDisplacement;
        private Vector3 arrowRotationAngles;
        private Vector3 childSpawnPosition;
        private Vector2 contentSizePanel;
        private Vector2 defaultContentPanelSizeRightHeight;
        private ControlScript mainScrip;
        private bool magazineRenderersReceived;
        private bool receiverRenderersReceived;
        private bool sightRenderersReceived;
        private bool stockRenderersReceived;
        private bool selectAllParts;
        private bool selectMagazineParts;
        private bool selectReceiverParts;
        private bool selectSightParts;
        private bool selectStockParts;
        private bool panelSwitchState;        
        private Ray ray;
        private RaycastHit raycastHitInfo;
        private GameObject hitObject;
        private GameObject allPartsArrow;
        private GameObject magazinePartsArrow;
        private GameObject receiverPartsArrow;
        private GameObject sightPartsArrow;
        private GameObject stockPartsArrow;
        private GameObject tempChildObject;
        //private GameObject contentPanelObject;
        private int magazineChildCount;
        private int receiverChildCount;
        private int sightChildCount;
        private int stockChildCount;
        private int magazineScrollHeightAdjust;
        private int receiverScrollHeightAdjust;
        private int sightScrollHeightAdjust;
        private int stockScrollHeightAdjust;
        private float totalScrollHeightAdjust;
        //private float defaultContentScrollHeight;
        //private GameObject tempArrowObject;
        private string hitObjectNameTag;
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        private PointerEventData pointerEventData;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private List<RaycastResult> results;
        private List<GameObject> magazineChildObjects;
        private List<GameObject> receiverChildObjects;
        private List<GameObject> sightChildObjects;
        private List<GameObject> stockChildObjects;
        private Image[] arrowImages;
        private RectTransform[] rectTransforms;
        private RectTransform contentPanelTransform;
        private bool AllPartsArrowDownwardState { get; set; }
        private bool MagazinePartsArrowDownwardState { get; set; }
        private bool ReceiverPartsArrowDownwardState { get; set; }
        private bool SightPartsArrowDownwardState { get; set; }
        private bool StockPartsArrowDownwardState { get; set; }
        //Transform[] gunChildObjects;
        // Start is called before the first frame update
        void Start()
        {
            mainScrip = GameObject.Find("Controller").GetComponent<ControlScript>();
            //Debug.Log(mainScrip.gunPrefab.gameObject.name);
            //State to check whether renderers are received
            magazineRenderersReceived = false;
            receiverRenderersReceived = false;
            sightRenderersReceived = false;
            stockRenderersReceived = false;

            //Set Panel Inactive            
            if (UIPanel.activeSelf)
                UIPanel.SetActive(false);
            
            //Retrieve the contents Rect Transform
            if (UIScrollPanel.activeSelf)
            {
                rectTransforms = UIScrollPanel.GetComponentsInChildren<RectTransform>();
                foreach (var item in rectTransforms)
                {
                    if (item.gameObject.name == "Content")
                        contentPanelTransform = item;
                }
                contentSizePanel = new Vector2(0,0);                
                defaultContentPanelSizeRightHeight = new Vector2(0, contentPanelTransform.rect.height);
                //Debug.Log(contentPanelTransform.rect.height);
                //defaultContentScrollHeight = contentPanelTransform.rect.height;
                UIScrollPanel.SetActive(false);
            }
                
            
            //Debug.Log(UIPanel.GetComponentInChildren<RectTransform>());
                       
            holdHitPos = new Vector3(0, 0, 0);
            //graphicRaycaster = UIOverallPartList.AddComponent<GraphicRaycaster>();
            graphicRaycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
            hitObjectNameTag = "";
            selectAllParts = false;
            selectMagazineParts = false;
            selectReceiverParts = false;
            selectSightParts = false;
            selectStockParts = false;
            //Debug.Log()
            UIPartListVecDistance = new Vector3(0, 0, 0);
            UIPartListVecDistance = UIReceiverPartList.transform.position - UIMagazinePartList.transform.position;
            UIPartChildListVecXDistance = new Vector3(10, 0, 0);
            //UIPartChildListVecDistance = UIPartListVecDistance;
            //UIPartChildListVecDistance.x = 20;
            panelSwitchState = false;
            AllPartsArrowDownwardState = false;
            MagazinePartsArrowDownwardState = false;
            ReceiverPartsArrowDownwardState = false;
            SightPartsArrowDownwardState = false;
            StockPartsArrowDownwardState = false;
            //allPartsArrow = GameObject.Find("ArrowImageAll").gameObject;
            //allPartsArrow = UIOverallPartList.GetComponentInChildren<Image>().gameObject;
            allPartsArrow = GetArrowObject(UIOverallPartList, "ArrowImageAll");
            magazinePartsArrow = GetArrowObject(UIMagazinePartList, "ArrowImageMag");
            receiverPartsArrow = GetArrowObject(UIReceiverPartList, "ArrowImageRec");
            sightPartsArrow = GetArrowObject(UISightPartList, "ArrowImageSight");
            stockPartsArrow = GetArrowObject(UIStockPartList, "ArrowImageStock");
            arrowDisplacement = new Vector3(-13, -1.5f, 0);
            arrowRotationAngles = new Vector3(0, 0, -90);
            //if(allPartsArrow == null)
            //{
            //    Debug.Log("No arrow detected");
            //}
            //else
            //{
            //    Debug.Log("All arrows detected");
            //}
            
            magazineChildCount = 0;
            magazineChildObjects = new List<GameObject>();            

            receiverChildCount = 0;
            receiverChildObjects = new List<GameObject>();

            sightChildCount = 0;
            sightChildObjects = new List<GameObject>();

            stockChildCount = 0;
            stockChildObjects = new List<GameObject>();

            childSpawnPosition = new Vector3(0, 0, 0);
            //Debug.Log(UITempTrial.GetComponent<TMPro.TextMeshProUGUI>().text);    
            magazineScrollHeightAdjust = 0;
            receiverScrollHeightAdjust = 0;
            sightScrollHeightAdjust = 0;
            stockScrollHeightAdjust = 0;
            totalScrollHeightAdjust = 0;
        }

        // Update is called once per frame
        void Update()
        {
            
            if(mainScrip.magazineMeshRenderers != null && !magazineRenderersReceived)
            {                
                magazineChildCount = mainScrip.magazineMeshRenderers.Length;
                magazineRenderersReceived = true;                
            }

            if (mainScrip.receiverMeshRenderers != null && !receiverRenderersReceived)
            {                
                receiverChildCount = mainScrip.receiverMeshRenderers.Length;                
                receiverRenderersReceived = true;
            }

            if (mainScrip.sightMeshRenderers != null && !sightRenderersReceived)
            {
                sightChildCount = mainScrip.sightMeshRenderers.Length;
                sightRenderersReceived = true;
            }

            if (mainScrip.stockMeshRenderers != null && !stockRenderersReceived)
            {
                stockChildCount = mainScrip.stockMeshRenderers.Length;
                stockRenderersReceived = true;
            }

            if (UIPanel.activeSelf)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Activating UI Hits");                    
                    ActivateOnHIt(Input.mousePosition);
                }
            }
            
        }

        void NameRetrieve(Renderer[] renderers)
        {
            foreach (var item in renderers)
            {
                Debug.Log(item.gameObject.name);
            }
        }

        void ActivateOnHIt(Vector3 hitPos)
        {
            //Debug.Log(hitPos);
            //ray = mainScrip.camera1.ScreenPointToRay(hitPos);
            //raycastHitInfo = Physics.RaycastAll(ray, 500);
            //pointerEventData.position = 
            //Debug.Log(raycastHitInfo.Length);
            //pointerEventData.position = UIOverallPartList.transform.position;
            //pointerEventData.position = UIPanel.transform.position;
            pointerEventData = new PointerEventData(eventSystem);
            results = new List<RaycastResult>();
            pointerEventData.position = hitPos;
            graphicRaycaster.Raycast(pointerEventData,results);
            //Debug.Log(results.Count);
            foreach (var item in results)
            {
                //Debug.Log(item.gameObject.name);     
                hitObjectNameTag = item.gameObject.tag;
               if(hitObjectNameTag == "AllParts" && !selectAllParts)
                {
                    //Debug.Log("All Parts");
                    OnAllPartSelected();
                    SetStateSelection(true, false, false, false, false);
                    mainScrip.PartTreeSelected(0);
                }
               if(hitObjectNameTag == "MagazineParts" && !selectMagazineParts)
                {
                    //Debug.Log("Magazine Parts");
                    OnMagazinePartSelected();
                    SetStateSelection(false, true, false, false, false);
                    mainScrip.PartTreeSelected(1);
                }
                if (hitObjectNameTag == "ReceiverParts" && !selectReceiverParts)
                {
                    //Debug.Log("Receiver Parts");
                    OnReceiverPartSelected();
                    SetStateSelection(false, false, true, false, false);
                    mainScrip.PartTreeSelected(2);
                }
                if (hitObjectNameTag == "SightParts" && !selectSightParts)
                {
                    //Debug.Log("Sight Parts");
                    OnSightPartSelected();
                    SetStateSelection(false, false, false, true, false);
                    mainScrip.PartTreeSelected(3);
                }
                if (hitObjectNameTag == "StockParts" && !selectStockParts)
                {
                    //Debug.Log("Stock Parts");
                    OnStockPartSelected();
                    SetStateSelection(false, false, false, false, true);
                    mainScrip.PartTreeSelected(4);
                }
            }
            SetStateSelection(false, false, false, false, false);
        }

        void SetStateSelection(bool allState, bool magazineState, bool receiverState, bool sightState, bool stockState)
        {
            selectAllParts = allState;
            selectMagazineParts = magazineState;
            selectReceiverParts = receiverState;
            selectSightParts = sightState;
            selectStockParts = stockState;
        }

        void OnAllPartSelected()
        {
            if (!AllPartsArrowDownwardState)
            {
                PartActiveState(true);
                ArrowRotateDownwardState(allPartsArrow, true);
                AllPartsArrowDownwardState = true;
            }
            else 
            {
                PartActiveState(false);
                ArrowRotateDownwardState(allPartsArrow, false);
                AllPartsArrowDownwardState = false;
            }           
        }

        void OnMagazinePartSelected()
        {            
            if (!MagazinePartsArrowDownwardState)
            {                
                UIReceiverPartList.transform.position = UIReceiverPartList.transform.position + (magazineChildCount) * UIPartListVecDistance;
                UISightPartList.transform.position = UISightPartList.transform.position + (magazineChildCount) * UIPartListVecDistance;
                UIStockPartList.transform.position = UIStockPartList.transform.position + (magazineChildCount) * UIPartListVecDistance;

                if (magazineChildObjects.Count == 0)
                {
                    InitializeSpawnNameObjects(mainScrip.magazineMeshRenderers, UIMagazinePartList.transform.position, UIMagazinePartList.transform,1);
                }
                else
                {
                    SpawnedObjectsState(magazineChildObjects, true);
                }                
                ArrowRotateDownwardState(magazinePartsArrow, true);
                MagazinePartsArrowDownwardState = true;
            }
            else
            {
                SpawnedObjectsState(magazineChildObjects, false);
                UIReceiverPartList.transform.position = UIReceiverPartList.transform.position - (magazineChildCount) * UIPartListVecDistance;
                UISightPartList.transform.position = UISightPartList.transform.position - (magazineChildCount) * UIPartListVecDistance;
                UIStockPartList.transform.position = UIStockPartList.transform.position - (magazineChildCount) * UIPartListVecDistance;
                ArrowRotateDownwardState(magazinePartsArrow, false);
                MagazinePartsArrowDownwardState = false;
            }
            SetScrollHeight();
        }

        void OnReceiverPartSelected()
        {
            if (!ReceiverPartsArrowDownwardState)
            {
                UISightPartList.transform.position = UISightPartList.transform.position + (receiverChildCount) * UIPartListVecDistance;
                UIStockPartList.transform.position = UIStockPartList.transform.position + (receiverChildCount) * UIPartListVecDistance;

                if (receiverChildObjects.Count == 0)
                {
                    InitializeSpawnNameObjects(mainScrip.receiverMeshRenderers, UIReceiverPartList.transform.position, UIReceiverPartList.transform,2);
                }
                else
                {
                    SpawnedObjectsState(receiverChildObjects, true);
                }
                ArrowRotateDownwardState(receiverPartsArrow, true);
                ReceiverPartsArrowDownwardState = true;
            }
            else
            {
                SpawnedObjectsState(receiverChildObjects, false);
                UISightPartList.transform.position = UISightPartList.transform.position - (receiverChildCount) * UIPartListVecDistance;
                UIStockPartList.transform.position = UIStockPartList.transform.position - (receiverChildCount) * UIPartListVecDistance;
                ArrowRotateDownwardState(receiverPartsArrow, false);
                ReceiverPartsArrowDownwardState = false;
            }
            SetScrollHeight();
        }

        void OnSightPartSelected()
        {
            if (!SightPartsArrowDownwardState)
            {                
                UIStockPartList.transform.position = UIStockPartList.transform.position + (sightChildCount) * UIPartListVecDistance;

                if (sightChildObjects.Count == 0)
                {
                    InitializeSpawnNameObjects(mainScrip.sightMeshRenderers, UISightPartList.transform.position, UISightPartList.transform, 3);
                }
                else
                {
                    SpawnedObjectsState(sightChildObjects, true);
                }
                ArrowRotateDownwardState(sightPartsArrow, true);
                SightPartsArrowDownwardState = true;
            }
            else
            {
                SpawnedObjectsState(sightChildObjects, false);           
                UIStockPartList.transform.position = UIStockPartList.transform.position - (sightChildCount) * UIPartListVecDistance;
                ArrowRotateDownwardState(sightPartsArrow, false);
                SightPartsArrowDownwardState = false;
            }
            SetScrollHeight();
        }

        void OnStockPartSelected()
        {
            if (!StockPartsArrowDownwardState)
            {              

                if (stockChildObjects.Count == 0)
                {
                    InitializeSpawnNameObjects(mainScrip.stockMeshRenderers, UIStockPartList.transform.position, UIStockPartList.transform, 4);
                }
                else
                {
                    SpawnedObjectsState(stockChildObjects, true);
                }
                ArrowRotateDownwardState(stockPartsArrow, true);
                StockPartsArrowDownwardState = true;
            }
            else
            {
                SpawnedObjectsState(stockChildObjects, false);                
                ArrowRotateDownwardState(stockPartsArrow, false);
                StockPartsArrowDownwardState = false;
            }
            SetScrollHeight();
        }

        GameObject GetArrowObject(GameObject parentObj, string arrowName)
        {
            arrowImages = parentObj.GetComponentsInChildren<Image>();
            foreach (var item in arrowImages)
            {
                if (item.gameObject.name == arrowName)
                    return item.gameObject;
            }

            return null;
        }

        void ArrowRotateDownwardState(GameObject arrowObj, bool state)
        {
            if (state)
            {
                arrowObj.transform.position = arrowObj.transform.position + arrowDisplacement;
                arrowObj.transform.Rotate(arrowRotationAngles);
            }
            else
            {
                arrowObj.transform.position = arrowObj.transform.position - arrowDisplacement;
                arrowObj.transform.Rotate(-arrowRotationAngles);
            }
        }

        void PartActiveState(bool partState)
        {
            UIMagazinePartList.SetActive(partState);
            UIReceiverPartList.SetActive(partState);
            UISightPartList.SetActive(partState);
            UIStockPartList.SetActive(partState);
        }

        void InitializeSpawnNameObjects(Renderer[] renderers, Vector3 parentPos, Transform parentTransform, int childRank)
        {
            int iterator = 1;
            foreach (var item in renderers)
            {
                //childSpawnPosition = parentPos + iterator * UIPartChildListVecDistance;
                childSpawnPosition = parentPos + iterator * UIPartListVecDistance + UIPartChildListVecXDistance;
                iterator++;
                tempChildObject = Instantiate(UIChildObjectPrefab,childSpawnPosition,Quaternion.identity, parentTransform);
                tempChildObject.name = item.gameObject.name;
                tempChildObject.GetComponent<TMPro.TextMeshProUGUI>().text = item.gameObject.name;
                switch (childRank)
                {
                    case 1:
                        magazineChildObjects.Add(tempChildObject);
                        break;
                    case 2:
                        receiverChildObjects.Add(tempChildObject);
                        break;
                    case 3:
                        sightChildObjects.Add(tempChildObject);
                        break;
                    case 4:
                        stockChildObjects.Add(tempChildObject);
                        break;
                    default:
                        break;
                }
                
            }
            
        }
        /*
        void RepositionSpawnedObjects(List<GameObject> gameObjects, Vector3 parentPos)
        {
            int iterator = 1;
            foreach (var item in gameObjects)
            {
                item.transform.position = parentPos + iterator * UIPartListVecDistance;
                iterator++;
            }
        }
        //*/
        void SpawnedObjectsState(List<GameObject> gameObjects, bool objectState)
        {
            foreach (var item in gameObjects)
            {
                item.SetActive(objectState);
            }            
        }

        void SetScrollHeight()
        {
            if (MagazinePartsArrowDownwardState)
                magazineScrollHeightAdjust = magazineChildCount;
            else
                magazineScrollHeightAdjust = 0;

            if (ReceiverPartsArrowDownwardState)
                receiverScrollHeightAdjust = receiverChildCount;
            else
                receiverScrollHeightAdjust = 0;

            if (SightPartsArrowDownwardState)
                sightScrollHeightAdjust = sightChildCount;
            else
                sightScrollHeightAdjust = 0;

            if (StockPartsArrowDownwardState)
                stockScrollHeightAdjust = stockChildCount;
            else
                stockScrollHeightAdjust = 0;

            totalScrollHeightAdjust = (10 + magazineScrollHeightAdjust + receiverScrollHeightAdjust + sightScrollHeightAdjust + stockScrollHeightAdjust) * 12.5f;
            if(totalScrollHeightAdjust < defaultContentPanelSizeRightHeight.y)
            {
                contentPanelTransform.sizeDelta = defaultContentPanelSizeRightHeight;
            }
            else
            {
                contentSizePanel.y = totalScrollHeightAdjust;
                contentPanelTransform.sizeDelta = contentSizePanel;
            }


        }
        public void PanelDisplay()
        {
            panelSwitchState = !UIPanel.activeSelf;
            if (panelSwitchState)
            {
                UIScrollPanel.SetActive(panelSwitchState);
                UIPanel.SetActive(panelSwitchState);
                UIOverallPartList.SetActive(panelSwitchState);
                PartActiveState(!panelSwitchState);
            }
            else
            {
                if (UIScrollPanel.activeSelf)
                {                    
                    UIPanel.SetActive(panelSwitchState);
                    UIScrollPanel.SetActive(panelSwitchState);
                }
                /*
                if (UIPanel.activeSelf)
                {
                    UIPanel.SetActive(panelSwitchState);
                }
                //*/
            }
        }

    }
}

