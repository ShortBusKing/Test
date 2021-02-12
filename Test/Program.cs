using System;
using System.Collections.Generic;
using System.Linq;

public class Test
{
    #region A nice Generic I stole from the Interwebs
    public static T GetPropertyValue<T>(object obj, string propName) { 
        return (T)obj.GetType().GetProperty(propName).GetValue(obj, null); 
    }
    #endregion

    #region Data Entities
    public class InventoryEntity
    {
        public int InventoryId { get; set; }
        public string InventoryDesc { get; set; }
        public int CurrentlyAvailable { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
    }

    public class OrderEntity
    {
        public int OrderId { get; set; }
        public bool InternationAddressFlag { get; set; }
        public int InventoryId { get; set; }
        public int RequiredInventoryCnt { get; set; }
        public List<int> Rules { get; set; }
        public bool Shipped { get; set; }
    }

    public class RuleDetailEntity
    {
        public int RuleId { get; set; }

        public string RuleName { get; set; }
        public int RulePriority { get; set; }
        public bool Enabled { get; set; }
        public string CompareA_ObjectName { get; set; }
        public string CompareA_Value { get; set; }
        public string CompareB_ObjectName { get; set; }
        public string CompareB_Value { get; set; }
        public string CompareType { get; set; }
    }

    public class SaleLogEntity
    {
        public int SaleId { get; set; }
        public List<OrderEntity> OrderList {get;set;}
        public List<InventoryEntity> SoldInventory {get; set;}

    }
  
    #endregion
    #region Inventory
    public static List<InventoryEntity> GetInventoryDetails()
    {
        List<InventoryEntity> i = new List<InventoryEntity>();
        i.Add(new InventoryEntity
        {
            InventoryId = 1,
            InventoryDesc = "Pet Rock",
            CurrentlyAvailable = 5,
            Height = 3,
            Width = 3,
            Length = 3
        });
        i.Add(new InventoryEntity
        {
            InventoryId = 2,
            InventoryDesc = "Bag of Rocks",
            CurrentlyAvailable = 0,
            Height = 1,
            Width = 1,
            Length = 1
        });
        i.Add(new InventoryEntity
        {
            InventoryId = 3,
            InventoryDesc = "Mixed Pebbles",
            CurrentlyAvailable = 0,
            Height = 1,
            Width = 1,
            Length = 1
        });

        i.Add(new InventoryEntity
        {
            InventoryId = 4,
            InventoryDesc = "Dirt",
            CurrentlyAvailable = 12,
            Height = 1,
            Width = 3,
            Length = 1
        });

        i.Add(new InventoryEntity
        {
            InventoryId = 5,
            InventoryDesc = "Quicksand",
            CurrentlyAvailable = 4,
            Height = 5,
            Width = 3,
            Length = 1
        });
        i.Add(new InventoryEntity
        {
            InventoryId = 6,
            InventoryDesc = "Imitation Quicksand",
            CurrentlyAvailable = 4,
            Height = 2,
            Width = 3,
            Length = 2
        });
        return i;
    }
    #endregion
    #region Order Details

    public static List<OrderEntity> GetOrderDetails()
    {
        List<int> RequiredRules = new List<int>();
        RequiredRules.Add(1);
        RequiredRules.Add(2);
        List<OrderEntity> o = new List<OrderEntity>();
        // Fail because of RequiredInventoryCnt 2 is more than 0 for InventoryId of 2
        o.Add(new OrderEntity
        {
            OrderId = 1,
            InternationAddressFlag = false,
            InventoryId = 1,
            RequiredInventoryCnt = 2,
            Rules = RequiredRules
        });
        o.Add(new OrderEntity
        {
            OrderId = 1,
            InternationAddressFlag = false,
            InventoryId = 2,
            RequiredInventoryCnt = 2,
            Rules = RequiredRules
        });
        // Fail
        o.Add(new OrderEntity
        {
            OrderId = 2,
            InternationAddressFlag = true,
            InventoryId = 1,
            RequiredInventoryCnt = 2,
            Rules = RequiredRules
        });
        // Pass
        o.Add(new OrderEntity
        {
            OrderId = 3,
            InternationAddressFlag = false,
            InventoryId = 1,
            RequiredInventoryCnt = 3,
            Rules = RequiredRules
        });
        // Fail not enough inventory
        o.Add(new OrderEntity
        {
            OrderId = 4,
            InternationAddressFlag = false,
            InventoryId = 1,
            RequiredInventoryCnt = 3,
            Rules = RequiredRules
        });
        return o;
    }
    #endregion
    #region Rule Details
    public static List<RuleDetailEntity> GetRuleDetails()
    {
        List<RuleDetailEntity> rd = new List<RuleDetailEntity>();
        rd.Add(new RuleDetailEntity
        {
            RuleId = 1,
            RuleName = "Internation Address Check" , 
            RulePriority = 1,
            Enabled = true,
            CompareA_ObjectName = "order",
            CompareA_Value = "InternationAddressFlag",
            CompareB_ObjectName = "bool",
            CompareB_Value = "true",
            CompareType = "Equals"
        });

        rd.Add(new RuleDetailEntity
        {
            RuleId = 2,
            RuleName = "Required Inventory Count Check",
            RulePriority = 1,
            Enabled = true,
            CompareA_ObjectName = "order",
            CompareA_Value = "RequiredInventoryCnt",
            CompareB_ObjectName = "inventory",
            CompareB_Value = "CurrentlyAvailable",
            CompareType = "GreaterThenOrEqual"
        });
        return rd;
    }
    #endregion

    public static void Main()
    {
        // You can start with just your list of orders, there's no need to implement a data layer or UI
        try
        {
            List<OrderEntity> orderDetails = GetOrderDetails();
            List<RuleDetailEntity> rules = GetRuleDetails();
            List<InventoryEntity> currentInventory = GetInventoryDetails();
            List<SaleLogEntity> soldItems = new List<SaleLogEntity>();
            List<OrderEntity> allValidOrders = new List<OrderEntity>();

            //var invalidOrders = orderCheck
            //        .Where(o => o.Value == false)
            //        .GroupBy(id => id.Key.OrderId)
            //        .Select(s => s.Key);

            //// filters out orders with part orders that fail the rule test
            //var validOrders = orderCheck
            //        .Where(o => invalidOrders.Contains(o.Key.OrderId))
            //        .Select(o => o.Key);

            //Process each order 
            foreach (int orderId in orderDetails.OrderBy(o => o.OrderId).Select(s => s.OrderId).Distinct().ToList())
            {
                // filters out orders with part orders that fail the rule test
                List<OrderEntity> orderList = orderDetails
                        .Where(o => orderId == o.OrderId)
                        .Select(o => o).ToList();

                // Save order information to Sales stack
                if (CheckOrder(orderList, currentInventory))
                {
                    allValidOrders.AddRange(orderList); 
                
                    // Process Inventory, Acts like a sales executtion
                    List<int> validOrderIds = orderList.Select(o => o.InventoryId).ToList();
                    List<InventoryEntity> soldItemsThisOrder = currentInventory.Where(i => validOrderIds.Contains(i.InventoryId)).Select(i => i).ToList();
                    
                    // Create Sale Log
                    SaleLogEntity saleLogEntity = new SaleLogEntity();
                    saleLogEntity.SaleId = orderId; // I'm just using any unique value... I would probably leverage an Identity column in the database in order to generate a value
                    saleLogEntity.OrderList = orderList;
                    saleLogEntity.SoldInventory = soldItemsThisOrder;
                    soldItems.Add(saleLogEntity);

                    // Remove Sold Items
                    // aka Update Inventory
                    foreach(OrderEntity oe in orderList)
                    {
                        if(currentInventory.Where(w => w.InventoryId == oe.InventoryId).Select(s => s.CurrentlyAvailable).FirstOrDefault() - oe.RequiredInventoryCnt >= 0)
                        {
                            foreach(InventoryEntity ie in currentInventory.Where(w => w.InventoryId == oe.InventoryId))
                            {
                                ie.CurrentlyAvailable = ie.CurrentlyAvailable - oe.RequiredInventoryCnt;
                            }
                        }
                        else
                        {
                            // the filter failed
                            throw new Exception("The Rules engine failed to check for enough inventory before processing Inv Cnt: "+ currentInventory[oe.InventoryId].CurrentlyAvailable + " Req Cnt: " + oe.RequiredInventoryCnt);
                        }
                    }
                }
            }

            // Process Sales output to screen
            string output = "";
            foreach(SaleLogEntity sold in soldItems)
            {
                foreach(OrderEntity si in sold.OrderList)
                {
                    output +=  sold.SoldInventory.Where(s=> s.InventoryId == si.InventoryId).Select(sel => sel.InventoryDesc).FirstOrDefault().ToString() + " @ " + si.RequiredInventoryCnt;
                }
            }
                

            Console.WriteLine("Sold Orders:" + soldItems.Count());
            Console.WriteLine("Sold Items List:" + output);
            
            Console.WriteLine("0");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static bool CheckOrder(List<OrderEntity> orderDetails, List<InventoryEntity> currentInventory)
    {
        //var invalidOrders = orderCheck
        //        .Where(o => o.Value == false)
        //        .GroupBy(id => id.Key.OrderId)
        //        .Select(s => s.Key);
        if(orderDetails.Count == 0)
        {
            return false;
        }

        bool PassedRuleCheck = true;
        foreach (OrderEntity order in orderDetails)
        {
            // Check all the rules
            List<RuleDetailEntity> rules = GetRuleDetails();

            foreach (int ruleId in order.Rules)
            {
                RuleDetailEntity ruleToCheck = rules.Where(r => r.RuleId == ruleId).First();
                if (ruleToCheck.Enabled == true && PassedRuleCheck == true)
                {
                    // this should be a function
                    switch (ruleToCheck.CompareType)
                    {
                        case "Equal":
                        case "Equals":
                            if (ruleToCheck.CompareA_ObjectName == "order") // I should have added a type instead of checking for specific names
                            {
                                if (ruleToCheck.CompareB_ObjectName == "bool")
                                {
                                    if (GetPropertyValue<bool>(order, ruleToCheck.CompareA_Value) == bool.Parse( ruleToCheck.CompareB_Value))
                                    {
                                        PassedRuleCheck = false;
                                    }
                                }
                                else if (ruleToCheck.CompareB_ObjectName.ToLower() == "inventory")
                                {
                                    if (GetPropertyValue<object>(order, ruleToCheck.CompareA_Value) == GetPropertyValue<object>(currentInventory.Where(i => i.InventoryId == order.InventoryId).FirstOrDefault(), ruleToCheck.CompareB_Value))
                                    {
                                        PassedRuleCheck = false;
                                    }
                                }
                            }
                            break;
                        case "LessThenOrEqual":
                            if (ruleToCheck.CompareA_ObjectName == "order") // I should have added a type instead of checking for specific names
                            {
                                if (ruleToCheck.CompareB_ObjectName.ToLower() == "inventory")
                                {
                                    if (GetPropertyValue<int>(order, ruleToCheck.CompareA_Value) >= GetPropertyValue<int>(currentInventory.Where(i => i.InventoryId == order.InventoryId).FirstOrDefault(), ruleToCheck.CompareB_Value))
                                    {
                                        PassedRuleCheck = false;
                                    }
                                }
                            }
                            break;
                        case "LessThen":
                            if (ruleToCheck.CompareA_ObjectName == "order") // I should have added a type instead of checking for specific names
                            {
                                if (ruleToCheck.CompareB_ObjectName.ToLower() == "inventory")
                                {
                                    if (GetPropertyValue<int>(order, ruleToCheck.CompareA_Value) > GetPropertyValue<int>(currentInventory.Where(i => i.InventoryId == order.InventoryId).FirstOrDefault(), ruleToCheck.CompareB_Value))
                                    {
                                        PassedRuleCheck = false;
                                    }
                                }
                            }
                            break;
                        case "GreaterThenOrEqual":
                            if (ruleToCheck.CompareA_ObjectName == "order")
                            {
                                if (ruleToCheck.CompareB_ObjectName.ToLower() == "inventory")
                                {
                                    if (GetPropertyValue<int>(order, ruleToCheck.CompareA_Value) >= GetPropertyValue<int>(currentInventory.Where(i => i.InventoryId == order.InventoryId).FirstOrDefault(), ruleToCheck.CompareB_Value))
                                    {
                                        PassedRuleCheck = false;
                                    }
                                }
                            }
                            break;
                        case "GreaterThen":
                            if (ruleToCheck.CompareA_ObjectName == "order")
                            {
                                if (ruleToCheck.CompareB_ObjectName.ToLower() == "inventory")
                                {
                                    if (GetPropertyValue<int>(order, ruleToCheck.CompareA_Value) > GetPropertyValue<int>(currentInventory.Where(i => i.InventoryId == order.InventoryId).FirstOrDefault(), ruleToCheck.CompareB_Value))
                                    {
                                        PassedRuleCheck = false;
                                    }
                                }
                            }
                            break;
                        default:
                            // Bad rules autofail
                            PassedRuleCheck = false;
                            break;
                            // More custom compare types can be build here. 
                            // This is nothing more than a simple check
                    }
 
                }
            }
        }
        return PassedRuleCheck;
    }

}
