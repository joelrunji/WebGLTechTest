using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace TechnicalTest
{
    public class ControlScript : MonoBehaviour
    {
        public GameObject gunPrefab;
        public GameObject partLabel;
        public Camera camera1;
        public Material magazineMaterial;
        public Material receiverMaterial;
        public Material sightMaterial;
        public Material stockMaterial;
        public Material tempOriginalMat;
        public Material tempSelectOrigMat;
        public Material xRayMat;
        public GameObject magazineObject { get; private set; }
        public Renderer[] magazineMeshRenderers { get; private set; }
        public Renderer[] receiverMeshRenderers { get; private set; }
        public Renderer[] sightMeshRenderers { get; private set; }
        public Renderer[] stockMeshRenderers { get; private set; }
        private Renderer[] meshNotXRay { get; set; }

        //public Camera cameraMain { get; private set; }

        //private Camera cam;
        private Vector3 camPos;
        private Vector3 spawnPos;
        private Vector3 rotAxis;
        private Vector3 prevMousePos;
        private Vector3 currentMousePos;
        private Vector3 tempExtentPos;
        private Vector3 selectObjOrigPos;
        private Vector3 partLabelDisplacement;
        private Vector3 tempRot;
        private Vector3 selectPartLabelPosition;
        private Vector3[] selectPartBoxColliderPositions;
        //private Vector3[] magazineBoxColliderLocalPositions;
        //private Vector3[] receiverBoxColliderLocalPositions;
        //private Vector3[] sightBoxColliderLocalPositions;
        //private Vector3[] stockBoxColliderLocalPositions;
        //private Vector3 rayHitPoint;
        private float rotDir;
        private Quaternion spawnRot;
        private GameObject gunObject;
        private Renderer[] magazineRenderers;
        private Renderer[] receiverRenderers;
        private Renderer[] sightRenderers;
        private Renderer[] stockRenderers;        
        private GameObject magazineObj;
        private GameObject receiverObj;
        private GameObject sightObj;
        private GameObject stockObj;
        private GameObject selectObj;
        private GameObject partLabelObj;
        private GameObject selectXRayPartObj;
        private Color highlightColor;
        private Color selectColor;
        private Ray ray;
        private Ray selectRay;
        private RaycastHit raycastHit;
        private RaycastHit raycastSelectHit;
        private Material tempMat;        
        private Material tempSelectMat;
        private Material defaultMagazineMaterial;
        private Material defaultReceiverMaterial;
        private Material defaultSightMaterial;
        private Material defaultStockMaterial;
        private MeshCollider tempCollider;
        private BoxCollider tempBoxCollider;
        //private BoxCollider[] magazineBoxColliders;
        //private BoxCollider[] receiverBoxColliders;
        //private BoxCollider[] sightBoxColliders;
        //private BoxCollider[] stockBoxColliders;
        private bool trackRotation;
        private bool trackHighlight;
        private bool trackMovement;
        private bool trackPartSelect;
        private bool trackPartRelease;
        private bool trackPartMove;
        private bool trackXRayMode;
        //private List<Vector3> initialPartPos;
        private Dictionary<GameObject, Vector3> partPositions;

        // Start is called before the first frame update
        void Start()
        {
            // Retrieve the camera position for initial central spawning            
            camPos = camera1.transform.position;
            spawnPos = camPos;
            spawnPos.z = 0;

            spawnRot = new Quaternion(0, 1f, 0, 1);
            gunObject = Instantiate(gunPrefab, spawnPos, spawnRot);

            //Add Components to Magazine
            magazineObj = GameObject.Find("Magazine");
            magazineRenderers = magazineObj.GetComponentsInChildren<MeshRenderer>();
            MaterialApplication(magazineMaterial,magazineRenderers);            
            MakeRigid(magazineRenderers);
            AddColliders(magazineRenderers);
            magazineObject = magazineObj;
            magazineMeshRenderers = magazineRenderers;
            magazineObj.tag = "MagazineFBXParts";
            TagChildObjects(magazineRenderers, "MagazineFBXParts");
            //magazineBoxColliders = magazineObj.GetComponentsInChildren<BoxCollider>();
            //magazineBoxColliderLocalPositions = new Vector3[magazineBoxColliders.Length];
            //ExtremeRightValMod(magazineBoxColliders);
            //foreach (var item in magazineRenderers)
            //{
            //    item.sharedMaterial = xRayMat;
            //    //Debug.Log(item.material.name);
            //}
            //Debug.Log(defaultMagazineMaterial.name);
            //string val = "";
            //foreach (var item in magazineBoxColliders)
            //{
            //    val = item.size.ToString() + ": " + item.center.ToString() + ": " + item.gameObject.transform.position.ToString();
            //    Debug.Log(val);
            //}
            //Debug.Log(magazineObj.transform.localPosition);

            //Add Components to Receiver
            receiverObj = GameObject.Find("Reciver");
            receiverRenderers = receiverObj.GetComponentsInChildren<MeshRenderer>();
            MaterialApplication(receiverMaterial, receiverRenderers);
            MakeRigid(receiverRenderers);
            AddColliders(receiverRenderers);
            receiverMeshRenderers = receiverRenderers;
            receiverObj.tag = "ReceiverFBXParts";
            TagChildObjects(receiverRenderers, "ReceiverFBXParts");
            //receiverBoxColliders = receiverObj.GetComponentsInChildren<BoxCollider>();

            //Add Components to Sight
            sightObj = GameObject.Find("Sight");
            sightRenderers = sightObj.GetComponentsInChildren<MeshRenderer>();
            MaterialApplication(sightMaterial, sightRenderers);
            MakeRigid(sightRenderers);
            AddColliders(sightRenderers);
            sightMeshRenderers = sightRenderers;
            sightObj.tag = "SightFBXParts";
            TagChildObjects(sightRenderers, "SightFBXParts");
            //sightBoxColliders = sightObj.GetComponentsInChildren<BoxCollider>();

            //Add Components to Stock
            stockObj = GameObject.Find("Stock");
            stockRenderers = stockObj.GetComponentsInChildren<MeshRenderer>();
            MaterialApplication(stockMaterial, stockRenderers);
            MakeRigid(stockRenderers);
            AddColliders(stockRenderers);
            stockMeshRenderers = stockRenderers;
            stockObj.tag = "StockFBXParts";
            TagChildObjects(stockRenderers, "StockFBXParts");
            //stockBoxColliders = stockObj.GetComponentsInChildren<BoxCollider>();

            highlightColor = new Color(0, 0.8f, 0);
            trackHighlight = true;
            StartCoroutine(HighlightMonitor());
            rotAxis = new Vector3(0, 0, 0);
            prevMousePos = new Vector3(0, 0, 0);
            currentMousePos = new Vector3(0, 0, 0);
            trackRotation = false;
            tempExtentPos = new Vector3(0, 0, 0);
            trackMovement = false;
            trackPartSelect = false;            
            selectColor = new Color(0.2f, 0.2f, 0.9f);
            selectObj = new GameObject("selection");
            trackPartRelease = false;
            selectObjOrigPos = new Vector3(0, 0, 0);
            tempRot = new Vector3(1, -1, 1);
            selectPartLabelPosition = new Vector3(0, 0, 0);
            //initialPartPos = new List<Vector3>() { magazineObj.transform.position, receiverObj.transform.position, sightObj.transform.position, stockObj.transform.position };
            partPositions = new Dictionary<GameObject, Vector3>() 
            {
                {magazineObj, magazineObj.transform.position } ,
                {receiverObj, receiverObj.transform.position } ,
                {sightObj, sightObj.transform.position } ,
                {stockObj, stockObj.transform.position } 
            };
            //tempSelectOrigMat.color = selectColor;
            //cameraMain = camera1;
            //partLabelObj = Instantiate(partLabelPrefab);
            partLabel.SetActive(false);
            partLabelDisplacement = new Vector3(100, 0, 0);
            //Employ original colors to save material
            ResetDefaultColorsOnPartSelection(false);
            defaultMagazineMaterial = magazineMaterial;
            defaultReceiverMaterial = receiverMaterial;
            defaultSightMaterial = sightMaterial;
            defaultStockMaterial = stockMaterial;
            //rayHitPoint = new Vector3(0, 0, 0);
            trackXRayMode = false;
            //ExtremeRightVal(magazineRenderers);   
            //selectPartMeshPositions = magazineObj.GetComponentsInChildren<>().;
            //Material properties
            
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(Input.mousePosition);
            currentMousePos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {                
                trackPartSelect = true;
                trackPartRelease = false;
                trackPartMove = false;
                PartSelect(Input.mousePosition);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                
                trackRotation = true;
                currentMousePos = Input.mousePosition;
                rotDir = currentMousePos.x - camera1.WorldToScreenPoint(spawnPos).x;
                //Debug.Log(camera1.WorldToScreenPoint(spawnPos));
                //Debug.Log(rotDir);
            }
            else if (Input.GetMouseButtonDown(2))
            {
                trackMovement = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //trackPartSelect = false;
                trackPartRelease = true;
            }
            else if (Input.GetMouseButtonUp(1))
            {                
                trackRotation = false;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                trackMovement = false;
            }
            tempExtentPos = currentMousePos - prevMousePos;
            if (trackRotation && prevMousePos != Vector3.zero)
            {
                //rotAxis = Vector3.Cross(Vector3.forward, tempExtentPos);
                //gunObject.transform.Rotate(Vector3.Normalize(rotAxis), tempExtentPos.magnitude * 0.5f);
                //*
                if (rotDir >= 0)
                {
                    rotAxis = Vector3.Cross(Vector3.forward, tempExtentPos);
                    gunObject.transform.Rotate(Vector3.Normalize(rotAxis), tempExtentPos.magnitude * 0.5f);
                }
                else
                {
                    rotAxis = Vector3.Cross(Vector3.back, tempExtentPos);
                    gunObject.transform.Rotate(Vector3.Normalize(Vector3.Scale(rotAxis,tempRot)), tempExtentPos.magnitude * 0.5f);
                }                
            }

            if(trackMovement && prevMousePos != Vector3.zero)
            {
                gunObject.transform.position = gunObject.transform.position + tempExtentPos*0.02f;
            }

            if(trackPartSelect && !trackPartRelease && prevMousePos != Vector3.zero)
            {
                if(selectObj == magazineObj || selectObj == receiverObj || selectObj == sightObj || selectObj == stockObj)
                {
                    selectObj.transform.position = selectObj.transform.position + tempExtentPos * 0.02f;
                    trackPartMove = true;
                    //*
                    if (!partLabel.activeSelf)
                        partLabel.SetActive(true);
                    //*/
                    UpdatePartLabelPosition(selectObj.transform.position);
                }
            }            

            prevMousePos = currentMousePos;
        }

        /*
        private void FixedUpdate()
        {
            ray = camera1.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out raycastHit, 500))
            {
                Debug.Log("Collision Detected");                
                tempMat = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().sharedMaterial;            

                if (tempMat.color != highlightColor)
                {
                    if (tempOriginalMat != tempMat)
                    {
                        Debug.Log("Entered");
                        if (magazineMaterial.color == highlightColor)
                            magazineMaterial.color = tempOriginalMat.color;
                        else if (receiverMaterial.color == highlightColor)
                            receiverMaterial.color = tempOriginalMat.color;
                        else if (sightMaterial.color == highlightColor)
                            sightMaterial.color = tempOriginalMat.color;
                        else if (stockMaterial.color == highlightColor)
                            stockMaterial.color = tempOriginalMat.color;
                        else
                            Debug.Log("None Found");
                    }

                    if (tempMat == magazineMaterial)
                    {
                        Debug.Log("Magazine Collision");
                        tempOriginalMat.color = magazineMaterial.color;
                        magazineMaterial.color = highlightColor;
                    }
                    else if (tempMat == receiverMaterial)
                    {
                        Debug.Log("Receiver Collision");
                        tempOriginalMat.color = receiverMaterial.color;
                        receiverMaterial.color = highlightColor;
                    }
                    else if (tempMat == sightMaterial)
                    {
                        Debug.Log("Sight Collision");
                        tempOriginalMat.color = sightMaterial.color;
                        sightMaterial.color = highlightColor;
                    }
                    else if (tempMat == stockMaterial)
                    {
                        Debug.Log("Stock Collision");
                        tempOriginalMat.color = stockMaterial.color;
                        stockMaterial.color = highlightColor;
                    }
                    else
                    {
                        Debug.Log("None Detected");
                    }
                } 
                
            }
            else
            {
                if (magazineMaterial.color == highlightColor)
                    magazineMaterial.color = tempOriginalMat.color;
                else if (receiverMaterial.color == highlightColor)
                    receiverMaterial.color = tempOriginalMat.color;
                else if (sightMaterial.color == highlightColor)
                    sightMaterial.color = tempOriginalMat.color;
                else if (stockMaterial.color == highlightColor)
                    stockMaterial.color = tempOriginalMat.color;
            }
        }
        //*/
        IEnumerator HighlightMonitor()
        {
            for(; ; )
            {
                ray = camera1.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit, 500))
                {
                    Debug.Log("Collision Detected");
                    tempMat = raycastHit.collider.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                                        
                    if (tempMat.color != highlightColor)
                    {
                        if (tempOriginalMat != tempMat)
                        {
                            Debug.Log("Entered");
                            if (magazineMaterial.color == highlightColor)
                                magazineMaterial.color = tempOriginalMat.color;
                            else if (receiverMaterial.color == highlightColor)
                                receiverMaterial.color = tempOriginalMat.color;
                            else if (sightMaterial.color == highlightColor)
                                sightMaterial.color = tempOriginalMat.color;
                            else if (stockMaterial.color == highlightColor)
                                stockMaterial.color = tempOriginalMat.color;
                            else
                                Debug.Log("None Found");
                        }

                        if (tempMat == magazineMaterial)
                        {
                            Debug.Log("Magazine Collision");
                            tempOriginalMat.color = magazineMaterial.color;
                            magazineMaterial.color = highlightColor;
                        }
                        else if (tempMat == receiverMaterial)
                        {
                            Debug.Log("Receiver Collision");
                            tempOriginalMat.color = receiverMaterial.color;
                            receiverMaterial.color = highlightColor;
                        }
                        else if (tempMat == sightMaterial)
                        {
                            Debug.Log("Sight Collision");
                            tempOriginalMat.color = sightMaterial.color;
                            sightMaterial.color = highlightColor;
                        }
                        else if (tempMat == stockMaterial)
                        {
                            Debug.Log("Stock Collision");
                            tempOriginalMat.color = stockMaterial.color;
                            stockMaterial.color = highlightColor;
                        }
                        else
                        {
                            Debug.Log("None Detected");
                        }
                    }

                }                
                else
                {                    
                        if (magazineMaterial.color == highlightColor)
                            magazineMaterial.color = tempOriginalMat.color;
                        else if (receiverMaterial.color == highlightColor)
                            receiverMaterial.color = tempOriginalMat.color;
                        else if (sightMaterial.color == highlightColor)
                            sightMaterial.color = tempOriginalMat.color;
                        else if (stockMaterial.color == highlightColor)
                            stockMaterial.color = tempOriginalMat.color;                    
                }

                yield return new WaitForSeconds(0.003f);
                if (!trackHighlight)
                    break;
            }            
        }
        
        void MaterialApplication(Material material, Renderer[] renderers)
        {
            foreach (var renderer in renderers)
            {
                renderer.material = material;
            }
        }

        void MakeRigid(Renderer[] renderers)
        {
            foreach (var item in renderers)
            {
                item.gameObject.AddComponent<Rigidbody>().isKinematic = true;
            }
        }

        void AddColliders(Renderer[] renderers)
        {
            foreach (var item in renderers)
            {
                tempCollider = item.gameObject.AddComponent<MeshCollider>();
                tempCollider.convex = true;
                tempCollider.isTrigger = true;
                //tempBoxCollider = item.gameObject.AddComponent<BoxCollider>();
            }
        }

        IEnumerator RotateModel()
        {
            for(; ; )
            {
                currentMousePos = Input.mousePosition;
                rotAxis = Vector3.Cross(Vector3.forward, currentMousePos - prevMousePos);
                Debug.Log(Vector3.Normalize(rotAxis));
                prevMousePos = currentMousePos;
                yield return new WaitForSeconds(0.005f);
                if (!trackRotation)
                    break;                
            }
            
        }

        //Method for part selection
        void PartSelect(Vector3 selectPos)
        {
            selectRay = camera1.ScreenPointToRay(selectPos);
            if (Physics.Raycast(selectRay,out raycastSelectHit, 500))
            {
                tempSelectMat = raycastSelectHit.collider.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                
                if (!trackXRayMode)
                {
                    //check whether the part is already selected
                    if (tempOriginalMat.color != selectColor)
                    {
                        // Checks whether we have selected a new part and resets previous selected part
                        if (tempSelectOrigMat != tempSelectMat)
                        {
                            Debug.Log("Enter Select");
                            /*
                            if (magazineMaterial.color == selectColor)
                                magazineMaterial.color = tempSelectOrigMat.color;
                            else if (receiverMaterial.color == selectColor)
                                receiverMaterial.color = tempSelectOrigMat.color;
                            else if (sightMaterial.color == selectColor)
                                sightMaterial.color = tempSelectOrigMat.color;
                            else if (stockMaterial.color == selectColor)
                                stockMaterial.color = tempSelectOrigMat.color;
                            else
                                Debug.Log("None Select Found");
                            //*/
                            ResetDefaultColorsOnPartSelection(true);
                        }
                        //Identifies selected parts to assign selection color.
                        if (tempSelectMat == magazineMaterial)
                        {
                            Debug.Log("Magazine Selection");
                            tempSelectOrigMat.color = tempOriginalMat.color;
                            magazineMaterial.color = selectColor;
                            selectObjOrigPos = magazineObj.transform.position;
                            selectObj = magazineObj;
                        }
                        else if (tempSelectMat == receiverMaterial)
                        {
                            Debug.Log("Receiver Selection");
                            tempSelectOrigMat.color = tempOriginalMat.color;
                            receiverMaterial.color = selectColor;
                            selectObjOrigPos = receiverObj.transform.position;
                            selectObj = receiverObj;
                        }
                        else if (tempSelectMat == sightMaterial)
                        {
                            Debug.Log("Sight Selection");
                            tempSelectOrigMat.color = tempOriginalMat.color;
                            sightMaterial.color = selectColor;
                            selectObjOrigPos = sightObj.transform.position;
                            selectObj = sightObj;
                        }
                        else if (tempSelectMat == stockMaterial)
                        {
                            Debug.Log("Stock Selection");
                            tempSelectOrigMat.color = tempOriginalMat.color;
                            stockMaterial.color = selectColor;
                            selectObjOrigPos = stockObj.transform.position;
                            selectObj = stockObj;
                        }
                        else
                        {
                            Debug.Log("None Selected");
                        }
                    }
                    /*
                    if(tempOriginalMat.color == selectColor)
                    {
                        if (magazineMaterial.color == selectColor)
                            magazineMaterial.color = tempSelectOrigMat.color;
                        else if (receiverMaterial.color == selectColor)
                            receiverMaterial.color = tempSelectOrigMat.color;
                        else if (sightMaterial.color == selectColor)
                            sightMaterial.color = tempSelectOrigMat.color;
                        else if (stockMaterial.color == selectColor)
                            stockMaterial.color = tempSelectOrigMat.color;

                        trackPartSelect = false;
                    }
                    //*/
                    //rayHitPoint = raycastSelectHit.transform.position;
                }
                else
                {
                    selectXRayPartObj = raycastSelectHit.collider.gameObject;

                    if (tempSelectMat == xRayMat)
                    {
                        //ResetIndividualPart2Xray();
                        if(meshNotXRay != null)
                        {
                            SetPartXRay(meshNotXRay, true, 0);
                        }
                        
                        if (selectXRayPartObj.tag == "MagazineFBXParts")
                        {
                            SetPartXRay(magazineRenderers, false, 1);
                            selectObjOrigPos = magazineObj.transform.position;
                            selectObj = magazineObj;
                            meshNotXRay = magazineRenderers;
                        }                            
                        else if (selectXRayPartObj.tag == "ReceiverFBXParts")
                        {
                            SetPartXRay(receiverRenderers, false, 2);
                            selectObjOrigPos = receiverObj.transform.position;
                            selectObj = receiverObj;
                            meshNotXRay = receiverRenderers;
                        }                            
                        else if (selectXRayPartObj.tag == "SightFBXParts")
                        {
                            SetPartXRay(sightRenderers, false, 3);
                            selectObjOrigPos = sightObj.transform.position;
                            selectObj = sightObj;
                            meshNotXRay = sightRenderers;
                        }                            
                        else if (selectXRayPartObj.tag == "StockFBXParts")
                        {
                            SetPartXRay(stockRenderers, false, 4);
                            selectObjOrigPos = stockObj.transform.position;
                            selectObj = stockObj;
                            meshNotXRay = stockRenderers;
                        }
                    }
                }
                
            }
            else
            {
                /*
                if (magazineMaterial.color == selectColor)
                    magazineMaterial.color = tempSelectOrigMat.color;
                else if (receiverMaterial.color == selectColor)
                    receiverMaterial.color = tempSelectOrigMat.color;
                else if (sightMaterial.color == selectColor)
                    sightMaterial.color = tempSelectOrigMat.color;
                else if (stockMaterial.color == selectColor)
                    stockMaterial.color = tempSelectOrigMat.color;

                trackPartSelect = false;
                //*/
                if (!trackXRayMode)
                {
                    ResetDefaultColorsOnPartSelection(false);
                }
                else
                {
                    if (meshNotXRay != null)
                    {
                        SetPartXRay(meshNotXRay, true, 0);                        
                    }
                    trackPartSelect = false;
                    if (partLabel.activeSelf)
                        partLabel.SetActive(false);
                }
            }
        }

        void ResetDefaultColorsOnPartSelection(bool partHit)
        {
            if (magazineMaterial.color == selectColor)
                magazineMaterial.color = tempSelectOrigMat.color;
            else if (receiverMaterial.color == selectColor)
                receiverMaterial.color = tempSelectOrigMat.color;
            else if (sightMaterial.color == selectColor)
                sightMaterial.color = tempSelectOrigMat.color;
            else if (stockMaterial.color == selectColor)
                stockMaterial.color = tempSelectOrigMat.color;

            if (!partHit)
            {
                trackPartSelect = false;
                if (partLabel.activeSelf)
                    partLabel.SetActive(false);
            }
        }

        void ResetPartOriginalPositionRotation()
        {
            foreach (var partObj in partPositions)
            {
                partObj.Key.transform.position = partObj.Value;
                partObj.Key.transform.rotation = spawnRot;
            }
        }
                
        void ExtremeRightVal(Renderer[] renderers)
        {
            /*
            var maxXVal =
                from renObjects in renderers
                let pos = renObjects.gameObject.transform.position
                orderby pos.x descending
                select pos[0];

            foreach (var item in maxXVal)
            {
                Debug.Log(item.ToString());
            }
            //*/
            string temp = "";
            //Debug.Log(maxXVal);
            foreach (var item in renderers)
            {
                temp = item.gameObject.transform.position.ToString() + ": " + item.gameObject.transform.localPosition;
                Debug.Log(temp);
            }
        }

        Vector3 ExtremeRightValMod(BoxCollider[] colliders)
        {
            selectPartBoxColliderPositions = new Vector3[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                selectPartBoxColliderPositions[i] = colliders[i].center + colliders[i].size;
                //Debug.Log(selectPartBoxColliderPositions[i].x);
            }
            var maxXLocalPos =
                from localPos in selectPartBoxColliderPositions                
                orderby localPos.x descending
                select localPos;
            //Debug.Log(maxXLocalPos.ElementAt(0));
            return camera1.WorldToScreenPoint(maxXLocalPos.ElementAt(0) + colliders[0].gameObject.transform.position);
        }

        void UpdatePartLabelPosition(Vector3 currentPartPosition)
        {
            /*
            if (selectObj == magazineObj )
            {                
                partLabel.GetComponent<TMPro.TextMeshProUGUI>().text = magazineObj.name;
            }
            else if (selectObj == receiverObj)
            {                
                partLabel.GetComponent<TMPro.TextMeshProUGUI>().text = receiverObj.name;
            }
            else if(selectObj == sightObj)
            {                
                partLabel.GetComponent<TMPro.TextMeshProUGUI>().text = sightObj.name;
            }
            else if(selectObj == stockObj)
            {                
                partLabel.GetComponent<TMPro.TextMeshProUGUI>().text = stockObj.name;
            }
            //*/
            if (selectObj == magazineObj)
            {
                partLabel.GetComponent<TMPro.TMP_InputField>().text = magazineObj.name;
                //selectPartLabelPosition = ExtremeRightValMod(magazineBoxColliders);
            }
            else if (selectObj == receiverObj)
            {
                partLabel.GetComponent<TMPro.TMP_InputField>().text = receiverObj.name;
                //selectPartLabelPosition = ExtremeRightValMod(receiverBoxColliders);
            }
            else if (selectObj == sightObj)
            {
                partLabel.GetComponent<TMPro.TMP_InputField>().text = sightObj.name;
                //selectPartLabelPosition = ExtremeRightValMod(sightBoxColliders);
            }
            else if (selectObj == stockObj)
            {
                partLabel.GetComponent<TMPro.TMP_InputField>().text = stockObj.name;
                //selectPartLabelPosition = ExtremeRightValMod(sightBoxColliders);
            }
            //partLabel.transform.position = currentPartPosition + partLabelDisplacement;
            //partLabel.transform.position = rayHitPoint + partLabelDisplacement;
            partLabel.transform.position = Input.mousePosition + partLabelDisplacement;
            //partLabel.transform.position = selectPartLabelPosition + partLabelDisplacement;
        }

        void SetPartXRay(Renderer[] renderers, bool state, int partNumber)
        {
            if (state)
            {
                foreach (var item in renderers)
                {
                    item.sharedMaterial = xRayMat;
                }
            }
            else
            {
                foreach (var item in renderers)
                {
                    switch (partNumber)
                    {
                        case 1:
                            item.sharedMaterial = defaultMagazineMaterial;
                            break;
                        case 2:
                            item.sharedMaterial = defaultReceiverMaterial;
                            break;
                        case 3:
                            item.sharedMaterial = defaultSightMaterial;
                            break;
                        case 4:
                            item.sharedMaterial = defaultStockMaterial;
                            break;
                        default:
                            break;
                    }
                }                
            }
        }

        void ResetIndividualPart2Xray() 
        {
            if (magazineMaterial != xRayMat)
                magazineMaterial = xRayMat;
            else if (receiverMaterial != xRayMat)
                receiverMaterial = xRayMat;
            else if (sightMaterial != xRayMat)
                sightMaterial = xRayMat;
            else if (stockMaterial != xRayMat)
                stockMaterial = xRayMat;
        }

        
        void TagChildObjects(Renderer[] renderers, string tagName)
        {
            foreach (var item in renderers)
            {
                item.tag = tagName;
            }
        }

        public void ResetButton()
        {
            ResetPartOriginalPositionRotation();
            ResetDefaultColorsOnPartSelection(false);
        }

        public void OnXRayClick()
        {
            trackXRayMode = !trackXRayMode;
            if (trackXRayMode)
            {
                ResetDefaultColorsOnPartSelection(false);                
            }
            SetPartXRay(magazineMeshRenderers, trackXRayMode, 1);
            SetPartXRay(receiverMeshRenderers, trackXRayMode, 2);
            SetPartXRay(sightMeshRenderers, trackXRayMode, 3);
            SetPartXRay(stockMeshRenderers, trackXRayMode, 4);
        }

        public void PartTreeSelected(int partNumber)
        {

            if (tempOriginalMat.color != selectColor)
            {
                //Refresh
                ResetDefaultColorsOnPartSelection(false);
                //Identifies selected parts to assign selection color.
                switch (partNumber)
                {
                    case 1:
                        Debug.Log("Magazine Selection");
                        tempSelectOrigMat.color = tempOriginalMat.color;
                        magazineMaterial.color = selectColor;
                        selectObjOrigPos = magazineObj.transform.position;
                        selectObj = magazineObj;
                        break;
                    case 2:
                        Debug.Log("Receiver Selection");
                        tempSelectOrigMat.color = tempOriginalMat.color;
                        receiverMaterial.color = selectColor;
                        selectObjOrigPos = receiverObj.transform.position;
                        selectObj = receiverObj;
                        break;
                    case 3:
                        Debug.Log("Sight Selection");
                        tempSelectOrigMat.color = tempOriginalMat.color;
                        sightMaterial.color = selectColor;
                        selectObjOrigPos = sightObj.transform.position;
                        selectObj = sightObj;
                        break;
                    case 4:
                        Debug.Log("Stock Selection");
                        tempSelectOrigMat.color = tempOriginalMat.color;
                        stockMaterial.color = selectColor;
                        selectObjOrigPos = stockObj.transform.position;
                        selectObj = stockObj;
                        break;
                    case 0:
                        ResetDefaultColorsOnPartSelection(false);
                        break;
                    default:
                        break;
                }
            }

        }

        private void OnDestroy()
        {
            trackHighlight = false;
            StopAllCoroutines();
            //StopCoroutine(HighlightMonitor());
        }
    }
}


