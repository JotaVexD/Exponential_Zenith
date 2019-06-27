﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using UnityEditor;

public class PlayerController : NetworkBehaviour
{
    // an inventory
    // action ability (interract with the world)

    
    // stats and skill levels
    public float speed;
    public const int m_MaxHealth = 100;
    private Rigidbody2D _rigidBody;

    //Inventory Panel
    public GameObject CharacterPanel;

    //Chest Panel
    public GameObject chestPanel;
    public GameObject Chest;
    public GameObject canvas;
    
    //Get Chest Itens and Show
    [SerializeField] Transform itemsParentChestSlot;
    public List<Item> itemsIn;
    [SerializeField] SlotChests[] itemChest;
    public bool InventoryOpen = false;
    private bool openChestOneTime;

    public event Action<ItemChest> OnRightClickEvent;
    [SerializeField] Inventory inventory;
    
    
    // [SyncVar(hook = nameof(OnChangeHealth))]
    public int m_CurrentHealth = m_MaxHealth;
    public Image healthBar;

    private void OnValidate(){
        if(itemsParentChestSlot != null){
            itemChest = itemsParentChestSlot.GetComponentsInChildren<SlotChests>();
        }
    }

    void Start() {
        _rigidBody = GetComponent<Rigidbody2D>();
        chestPanel.SetActive(false);
    }

    void FixedUpdate() {

        if(!isLocalPlayer){
            return;
        }

        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Player/Races/"+CharConfig.Instance.charData.race);
        // Change Color by class 
        if(CharConfig.Instance.charData.classe == "Guardian"){
            this.GetComponent<SpriteRenderer>().color = new Color(0f,0.7371f,0.5607f);
        }else if(CharConfig.Instance.charData.classe == "Samurai"){
            this.GetComponent<SpriteRenderer>().color = new Color(0.8901f,0f,0f);
        }else if(CharConfig.Instance.charData.classe == "Monk"){
            this.GetComponent<SpriteRenderer>().color = new Color(0.9372f,0.8274f,0f);            
        }

        canvas.SetActive(true);
        var mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x, 
            mousePosition.y - transform.position.y
        );

        transform.up = direction;


        _rigidBody.position += Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        _rigidBody.position += Vector2.up * Input.GetAxis("Vertical") * speed * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("Dano");
            // CmdTakeHealth(10);
        }

        if(Input.GetKeyDown(KeyCode.I) && InventoryOpen == false){
            CharacterPanel.SetActive(true);
            InventoryOpen = true;
        }else if(Input.GetKeyDown(KeyCode.I) && InventoryOpen == true){
            CharacterPanel.SetActive(false);
            InventoryOpen = false;
        }
    }

    

    private void OnTriggerStay2D(Collider2D other) {
        if(!isLocalPlayer){
            return;
        }
        if(other.gameObject.CompareTag("Chest") && Input.GetKeyDown(KeyCode.E)){
            CharacterPanel.SetActive(true);
            chestPanel.SetActive(true);
            openChestOneTime = true;
            Chest = other.gameObject;
            List<Item> itemsInThere = other.GetComponent<ItemChest>().itemsInThere;
            for(int i = 0; i < itemsInThere.Count && i < itemChest.Length; i++){
                itemChest[i].Item = itemsInThere[i];
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(!isLocalPlayer){
            return;
        }
        if(openChestOneTime == true){
            CharacterPanel.SetActive(false);
            chestPanel.SetActive(false);
            Chest = null;
            GameObject ChestOpen = GetComponent<Character>().ChestOpen;
            ChestOpen = null;
            for(int i = 0; i < itemChest.Length; i++){
                itemChest[i].gameObject.GetComponent<Image>().color = new Color(0,0,0,0);
                itemChest[i].gameObject.GetComponent<Image>().sprite = null;
            }
        }
    }
}