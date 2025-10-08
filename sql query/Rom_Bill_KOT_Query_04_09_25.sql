update tbl_config_mast set print_cap = 2,SingleKOT = 1,NoofPrint_KOT_DN = 1, NoofPrint_KOT_HD = 1, NoofPrint_KOT_TA = 1


IF Not EXISTS(SELECT * FROM sys.columns 
          WHERE Name = N'isPrintZomatoIdOnKotHD' AND Object_ID = Object_ID(N'tbl_Config_Mast'))
BEGIN
alter table tbl_Config_Mast add isPrintZomatoIdOnKotHD tinyint
END
Go 


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_Get_Item_KOT_Counter_MP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Usp_Get_Item_KOT_Counter_MP 
Go
Create proc [dbo].Usp_Get_Item_KOT_Counter_MP   --@Bill_No_FK='9219',@counter_id='1'                              
(                                  
@Bill_No_FK varchar(50),                               
@counter_id bigint                  
)                                  
as                                  
begin                                  
select a.id,  
substring(a.i_name,1,35)'i_name',  
item_code,qty,convert(varchar,amount,1) as amount,                                
(case (bt.Comments) when 'null' then '' when null then '' else bt.Comments end) Comments,                    
(case(item_addon) when ' ' then '0' else item_addon end) item_addon,isnull(Group_Dish,0) Group_Dish,        
isnull(Urgent_Item,0) as 'Urgent_Item',addon_index,isnull(a.kot_flag,0) as 'kot_flag'   
,a.POS_Desc as I_Name_Desc,  
(case when Channel_Order_Id like '%-%' then   
 ( left(Channel_Order_Id, charindex('-', Channel_Order_Id) - 1))  
else   
Channel_Order_Id end)  
 as zomato_order_id           
from tbl_bill_tran bt WITH (NOLOCK) 
inner join tbl_bill bm with (nolock) on  bt.Bill_No_FK=bm.Bill_No                                   
inner join tbl_dish_mast a on item_code=a.i_code and bill_no_fk=@Bill_No_FK           
and a.counter_id=@counter_id    and upper(a.i_name)<>'NA' and isnull(addon_index_fk,'0')='0'      
order by bt.id          
end  
Go



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_GetRetailInvoice_Print_2]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Usp_GetRetailInvoice_Print_2] 
Go
create PROC [dbo].[Usp_GetRetailInvoice_Print_2] --9220
(
  @BillNo BIGINT
)
AS
BEGIN
    DECLARE @Cust_Code VARCHAR(20),
            @PTRN VARCHAR(20),
            @IsShowFullItemNameOnBill VARCHAR(20),
            @Initial VARCHAR(10);
    -- Get config values
    SET @Cust_Code = (SELECT cust_code FROM tbl_bill WHERE bill_no = @BillNo);
    SET @IsShowFullItemNameOnBill = (SELECT IsShowFullItemNameOnBill FROM tbl_Config_Mast);
    SET @Initial = (SELECT Initial FROM tbl_Config_Mast);
    -- Bill Header
    SELECT 
        (SELECT TOP 1 
            (CASE a.payment_mode WHEN 3 THEN 'Card Type :' + card_type ELSE '' END)
         FROM tbl_Payment_Settlement a WITH (NOLOCK) 
         WHERE bill_no_fk = tbl_bill.Bill_No) AS card_type,
        (SELECT Name FROM TBL_CustomerInfo WHERE id = tbl_Bill.Cust_Code) AS custname,
        (SELECT Mobile_No FROM TBL_CustomerInfo WHERE id = tbl_Bill.Cust_Code) AS custmobile,
        (SELECT Address FROM TBL_CustomerInfo WHERE id = tbl_Bill.Cust_Code) AS custadd,
        *,
        @Initial AS Initial,
        CONVERT(VARCHAR(10), Bill_Date, 105) + ' ' + 
        (SELECT LTRIM(RIGHT(CONVERT(VARCHAR(20), FORMAT(createdon,'MM/dd/yyyy hh:mm tt'), 100), 8))
         FROM tbl_bill WHERE Bill_No = @BillNo) AS Bill_Date_display
    FROM tbl_Bill 
    WHERE Bill_No = @BillNo;
-- Step 0: Temp table for final result
IF OBJECT_ID('tempdb..#FinalResult') IS NOT NULL DROP TABLE #FinalResult;
CREATE TABLE #FinalResult (
    I_Name NVARCHAR(255),
    Qty INT,
    Rate DECIMAL(18,2),
    Amount DECIMAL(18,2),
    dishComments NVARCHAR(MAX),
    item_index VARCHAR(50),
    Step_index VARCHAR(50),
    sno INT,
    SourceOrder INT
);
-- Step 1: Part A - Main & Add-On Items from tbl_Bill_Tran
INSERT INTO #FinalResult
SELECT   
    CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No=@BillNo AND i_code_fk=b.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk=b.I_Code) THEN ''
        --WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No=@BillNo AND i_code_fk=b.I_Code) THEN ''
        ELSE  
            CASE ISNULL(@IsShowFullItemNameOnBill, 0)
                WHEN 1 THEN CASE ISNULL(addon_index_fk, '0') WHEN '0' THEN b.I_Name ELSE '+ ' + b.I_Name END
                WHEN 0 THEN CASE ISNULL(addon_index_fk, '0') WHEN '0' THEN SUBSTRING(b.I_Name, 1, 21) ELSE SUBSTRING('+ ' + b.I_Name, 1, 21) END
                ELSE SUBSTRING(', ' + b.I_Name, 1, 21)
            END
    END AS I_Name,
    CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk = b.I_Code) THEN 0
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code) THEN 0
        ELSE SUM(a.qty)
    END AS Qty,
    CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk = b.I_Code) THEN 0
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code) THEN 0
        ELSE a.Rate
    END AS Rate,
    CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk = b.I_Code) THEN 0
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code) THEN 0
        ELSE SUM(a.Amount)
    END AS Amount,
    CASE WHEN ISNULL(sdm.SUD_dept, '') = 'COMBO' THEN '(' + b.I_Name_Desc + ')' ELSE a.Comments END AS dishComments,
    CASE 
        WHEN addon_index_fk = '0' AND a.Group_Dish = '0' THEN addon_index 
        WHEN addon_index_fk != '0' THEN addon_index_fk 
        ELSE a.Group_Dish 
    END AS item_index,
    '' AS Step_index,
    CASE WHEN addon_index_fk != '0' THEN 100 ELSE -1 END AS sno,
    2 AS SourceOrder
FROM tbl_Bill_Tran a
INNER JOIN tbl_Dish_Mast b ON a.Item_Code = b.I_Code
INNER JOIN tbl_Department_Mast dm ON b.deptt_id = dm.id
INNER JOIN tbl_Super_depart_Mast sdm ON sdm.id = dm.SUP_ID
WHERE a.Bill_No_FK = @BillNo AND a.qty <> 0
GROUP BY b.I_Name, a.Rate, a.Comments, a.Group_Dish, b.I_Code, sdm.SUD_dept, a.addon_index_fk, a.addon_index, b.I_Name_Desc;
-- Step 2: Create grouped item names by item_index and qty
IF OBJECT_ID('tempdb..#GroupedAssortedItems') IS NOT NULL DROP TABLE #GroupedAssortedItems;
SELECT 
    a.item_index,
    a.Qty,
    STUFF((
        SELECT ' . ' + (b2.I_Name)
        FROM tbl_assorted_item a2
        INNER JOIN tbl_Dish_Mast b2 ON a2.I_Code = b2.I_Code
        WHERE a2.Bill_No = @BillNo
          AND a2.item_index = a.item_index
          AND a2.Qty = a.Qty
          AND (b2.I_Name) <> 'NA'
          AND b2.I_Name NOT LIKE '%WHITE BUN%'
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 3, '') AS I_Name
INTO #GroupedAssortedItems
FROM tbl_assorted_item a
INNER JOIN tbl_Dish_Mast b ON a.I_Code = b.I_Code
WHERE a.Bill_No = @BillNo
  AND (b.I_Name) <> 'NA'
  AND b.I_Name NOT LIKE '%WHITE BUN%'
  AND b.I_Name NOT LIKE '%. Done%' 
GROUP BY a.item_index, a.Qty;
-- Step 3: Get rate/amount once per item_index from tbl_Bill_Tran
IF OBJECT_ID('tempdb..#RateAmountByItemIndex') IS NOT NULL DROP TABLE #RateAmountByItemIndex;
SELECT 
    CASE 
        WHEN ISNULL(addon_index_fk, '0') = '0' AND ISNULL(Group_Dish, '0') = '0' THEN addon_index 
        WHEN ISNULL(addon_index_fk, '0') != '0' THEN addon_index_fk 
        ELSE Group_Dish 
    END AS item_index,
    MAX(Rate) AS Rate,
    SUM(Amount) AS Amount
INTO #RateAmountByItemIndex
FROM tbl_Bill_Tran
WHERE Bill_No_FK = @BillNo
GROUP BY 
    CASE 
        WHEN ISNULL(addon_index_fk, '0') = '0' AND ISNULL(Group_Dish, '0') = '0' THEN addon_index 
        WHEN ISNULL(addon_index_fk, '0') != '0' THEN addon_index_fk 
        ELSE Group_Dish 
    END;
-- Step 4: Insert Part B (grouped assorted items by qty)
-- Step 4: Insert Part B (grouped assorted items by qty, fixed with ROW_NUMBER subquery)
INSERT INTO #FinalResult
SELECT 
    '. ' + I_Name AS I_Name,
    Qty,
    CASE WHEN I_Name NOT LIKE '%. Done%' THEN Rate ELSE 0 END AS Rate,
    CASE WHEN I_Name NOT LIKE '%. Done%' THEN Amount ELSE 0 END AS Amount,
    '' AS dishComments,
    CAST(item_index AS VARCHAR),
    '' AS Step_index,
    -1 AS sno,
    1 AS SourceOrder
FROM (
    SELECT 
        g.I_Name,
        g.Qty,
        g.item_index,
        ISNULL(r.Rate, 0) AS Rate,
        ISNULL(r.Amount, 0) AS Amount,
        ROW_NUMBER() OVER (PARTITION BY g.item_index ORDER BY g.Qty) AS rn
    FROM #GroupedAssortedItems g
    LEFT JOIN #RateAmountByItemIndex r ON r.item_index = g.item_index
) AS t
-- Step 5: Final Output
SELECT * FROM #FinalResult 
WHERE Qty <> -1 AND I_Name NOT LIKE '%. Done%' 
ORDER BY item_index, SourceOrder DESC;
-- Cleanup
DROP TABLE #FinalResult;
DROP TABLE #GroupedAssortedItems;
DROP TABLE #RateAmountByItemIndex;

    -- Taxes
    EXEC Usp_GetAllTaxCrystal @BillNo = @BillNo;
    -- Discount Summary
    SELECT DISTINCT 
        'Discount on ' + CAST(c.cat_name AS VARCHAR) + ' @' AS Category,
        ISNULL(SUM(a.dis_amount), 0) AS Dis_Amount,
        a.bill_no_fk
    FROM tbl_bill_tran a
    INNER JOIN tbl_dish_mast b ON a.item_code = b.i_code
    INNER JOIN tbl_category_dish_mast c ON b.cat_id = c.id
    WHERE a.bill_no_fk = @BillNo AND a.dis_amount > 0
    GROUP BY c.cat_name, a.bill_no_fk;
END
Go




IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_GetAllTaxCrystal]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Usp_GetAllTaxCrystal] 
Go
CREATE Proc [dbo].[Usp_GetAllTaxCrystal] --9212           
(              
@BillNo bigint              
)              
as              
begin              
--===============================================              
----Tax Cap----------------              
declare @ServiceTaxCap varchar(10)              
declare @SurchargeCap varchar(10)              
declare @SBCCap varchar(10)            
declare @KKCCap varchar(10)             
declare @DeliveryCap varchar(10)            
----Tax Amount Variable----------------              
declare @Dis_Amount money              
declare @Servicetax_Amount money              
declare @Surcharge money              
declare @SBC_Amount money            
declare @KKC_Amount money               
declare @Delivery_charge money              
declare @Roundoff money              
--======================create temp table==========================              
create table #tbl_temp(tax varchar(max),Tax_Amount money,bill_no_fk bigint,SLno int)              
--set value              
select @Dis_Amount=isnull(dis_amount,0),@Servicetax_Amount=Service_Tax,@Surcharge=sur_charge,@SBC_Amount=SBC_Tax,            
@Roundoff=RoundOff,@Delivery_charge=isnull(Other_Charge,0),@KKC_Amount=isnull(KKC_Tax,0) from tbl_bill where bill_no=@BillNo              
--set tax cap              
select @ServiceTaxCap=Service_tax,@SurchargeCap=Sur_charge,@SBCCap=SBC_Tax,@KKCCap=KKC_Tax,@DeliveryCap=DeliveryCharge from tbl_config_mast              
-- discount                      
if(@Dis_Amount>0)              
insert into #tbl_temp(tax,Tax_Amount,bill_no_fk,SLno)values('Discount @ :',@Dis_Amount,@BillNo,0)              
--del charge      
if(@Delivery_charge>0)            
insert into #tbl_temp(tax,Tax_Amount,bill_no_fk,SLno)values('Pkg/del Charge@'+@DeliveryCap+'% :',@Delivery_charge,@BillNo,1)                 
-- VAT              
--insert into #tbl_temp              
--select distinct 'Vat@'+cast(a.tax as varchar)+'% : ' tax,Isnull(sum(b.Tax_Amount),0) 'Tax_Amount',bill_no_fk                                            
--from tbl_dish_mast a                                        
--inner join tbl_bill_tran b                                      
--on a.i_code=b.item_code where bill_no_fk=@BillNo  and b.Tax_Amount>0                                      
--group by a.tax,bill_no_fk  
  insert into #tbl_temp(tax,Tax_Amount,bill_no_fk,SLno)
              SELECT  a.Tax_display_name+' :' as tax,sum(a.Tax_Amt) as tax_amount,a.bill_no_fk,3 as SLno FROM tbl_bill_tax a  
   WHERE a.Bill_No_FK  =@BillNo and Tax_Amt>0  
  group by Tax_display_name,a.bill_no_fk  
---- Service_Tax                      
--if(@Servicetax_Amount>0)              
--insert into #tbl_temp(tax,Tax_Amount,bill_no_fk)values('Service Tax@'+@ServiceTaxCap+'% :',@Servicetax_Amount,@BillNo)              
----SBC Tax              
--if(@SBC_Amount>0)              
--insert into #tbl_temp(tax,Tax_Amount,bill_no_fk)values('SBC@'+@SBCCap+'% :',@SBC_Amount,@BillNo)             
----KKCC Tax              
--if(@KKC_Amount>0)              
--insert into #tbl_temp(tax,Tax_Amount,bill_no_fk)values('KKC@'+@KKCCap+'% :',@KKC_Amount,@BillNo)             
---- Surcharge                    
--if(@Surcharge>0)              
--insert into #tbl_temp(tax,Tax_Amount,bill_no_fk)values('Surcharge@'+@SurchargeCap+'% on vat :',@Surcharge,@BillNo)              
--round off             
insert into #tbl_temp(tax,Tax_Amount,bill_no_fk,SLno)values('       R. Off :',@Roundoff,@BillNo,10)              
---select               
select *from #tbl_temp              
drop table #tbl_temp              
end
Go




IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_GetRetailInvoice_Print_HD_New]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Usp_GetRetailInvoice_Print_HD_New] 
Go
create proc [dbo].[Usp_GetRetailInvoice_Print_HD_New] --'9222',1455737      
(                       
  @BillNo bigint,                 
  @Cust_Code bigint                       
)                       
as        
declare @IsShowFullItemNameOnBill varchar(20)     ,@Initial varchar(100)                                
set @IsShowFullItemNameOnBill = (select IsShowFullItemNameOnBill from tbl_Config_Mast)    
   set @Initial=(select Initial from tbl_Config_Mast) 
select 
 REPLACE(REPLACE(REPLACE(web_order_comments,'''',''), CHAR(13), ''), CHAR(10), '')  as web_order_comments,  
*,(Select c.Card_type from tbl_card_type_mast c where c.Id = source_of_order) as [OrderSource] ,      
case(payment_mode) when 1 then            
(case(isnull(Web_order_no,0)) when 0 THEN CASE(ISBILLSATTLE) WHEN 0 THEN 'COD' ELSE 'CARD:'+CARD_TYPE END else  'COD' END) when  3 then          
 (case(isnull(Web_order_no,0)) when 0 then 'CARD:'+CARD_TYPE else 'ONLINE PAID'  END)      
WHEN 2 THEN 'CHEQUE:'+CARD_TYPE WHEN 4 THEN 'COUPON:'+CARD_TYPE       
when 0 then      
(case(isnull(Web_order_no,0)) when 0 THEN CASE(ISBILLSATTLE) WHEN 0 THEN 'COD' ELSE 'CARD:'+CARD_TYPE END else  'COD' END) when  3 then          
 (case(isnull(Web_order_no,0)) when 0 then 'CARD:'+CARD_TYPE else 'ONLINE PAID'  END)      
ELSE  'OTHER' end as 'PayMode'    ,@Initial as  Initial 
  ,convert(varchar(10),Bill_Date,105) +' ' +  (select LTRIM(RIGHT(CONVERT(VARCHAR(20), format(createdon,'MM/dd/yyyy hh:mm tt') , 100), 8))  from tbl_bill where Bill_No=@BillNo) as  Bill_Date_display
 from tbl_Bill where Bill_No=@BillNo     
 
 
 
-- Step 0: Temp table for final result
IF OBJECT_ID('tempdb..#FinalResult') IS NOT NULL DROP TABLE #FinalResult;
CREATE TABLE #FinalResult (
    I_Name NVARCHAR(255),
    Qty INT,
    Rate DECIMAL(18,2),
    Amount DECIMAL(18,2),
    dishComments NVARCHAR(MAX),
    item_index VARCHAR(50),
    Step_index VARCHAR(50),
    sno INT,
    SourceOrder INT
);
-- Step 1: Part A - Main & Add-On Items from tbl_Bill_Tran
INSERT INTO #FinalResult
SELECT   
    CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No=@BillNo AND i_code_fk=b.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk=b.I_Code) THEN ''
        --WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No=@BillNo AND i_code_fk=b.I_Code) THEN ''
        ELSE  
            CASE ISNULL(@IsShowFullItemNameOnBill, 0)
                WHEN 1 THEN CASE ISNULL(addon_index_fk, '0') WHEN '0' THEN b.I_Name ELSE '+ ' + b.I_Name END
                WHEN 0 THEN CASE ISNULL(addon_index_fk, '0') WHEN '0' THEN SUBSTRING(b.I_Name, 1, 21) ELSE SUBSTRING('+ ' + b.I_Name, 1, 21) END
                ELSE SUBSTRING(', ' + b.I_Name, 1, 21)
            END
    END AS I_Name,
    CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk = b.I_Code) THEN 0
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code) THEN 0
        ELSE SUM(a.qty)
    END AS Qty,
    CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk = b.I_Code) THEN 0
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code) THEN 0
        ELSE a.Rate
    END AS Rate,
    CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk = b.I_Code) THEN 0
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No = @BillNo AND i_code_fk = b.I_Code) THEN 0
        ELSE SUM(a.Amount)
    END AS Amount,
    CASE WHEN ISNULL(sdm.SUD_dept, '') = 'COMBO' THEN '(' + b.I_Name_Desc + ')' ELSE a.Comments END AS dishComments,
    CASE 
        WHEN addon_index_fk = '0' AND a.Group_Dish = '0' THEN addon_index 
        WHEN addon_index_fk != '0' THEN addon_index_fk 
        ELSE a.Group_Dish 
    END AS item_index,
    '' AS Step_index,
    CASE WHEN addon_index_fk != '0' THEN 100 ELSE -1 END AS sno,
    2 AS SourceOrder
FROM tbl_Bill_Tran a
INNER JOIN tbl_Dish_Mast b ON a.Item_Code = b.I_Code
INNER JOIN tbl_Department_Mast dm ON b.deptt_id = dm.id
INNER JOIN tbl_Super_depart_Mast sdm ON sdm.id = dm.SUP_ID
WHERE a.Bill_No_FK = @BillNo AND a.qty <> 0
GROUP BY b.I_Name, a.Rate, a.Comments, a.Group_Dish, b.I_Code, sdm.SUD_dept, a.addon_index_fk, a.addon_index, b.I_Name_Desc;
-- Step 2: Create grouped item names by item_index and qty
IF OBJECT_ID('tempdb..#GroupedAssortedItems') IS NOT NULL DROP TABLE #GroupedAssortedItems;
SELECT 
    a.item_index,
    a.Qty,
    STUFF((
        SELECT ' . ' + (b2.I_Name)
        FROM tbl_assorted_item a2
        INNER JOIN tbl_Dish_Mast b2 ON a2.I_Code = b2.I_Code
        WHERE a2.Bill_No = @BillNo
          AND a2.item_index = a.item_index
          AND a2.Qty = a.Qty
          AND (b2.I_Name) <> 'NA'
          AND b2.I_Name NOT LIKE '%WHITE BUN%'
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 3, '') AS I_Name
INTO #GroupedAssortedItems
FROM tbl_assorted_item a
INNER JOIN tbl_Dish_Mast b ON a.I_Code = b.I_Code
WHERE a.Bill_No = @BillNo
  AND (b.I_Name) <> 'NA'
  AND b.I_Name NOT LIKE '%WHITE BUN%'
  AND b.I_Name NOT LIKE '%. Done%' 
GROUP BY a.item_index, a.Qty;
-- Step 3: Get rate/amount once per item_index from tbl_Bill_Tran
IF OBJECT_ID('tempdb..#RateAmountByItemIndex') IS NOT NULL DROP TABLE #RateAmountByItemIndex;
SELECT 
    CASE 
        WHEN ISNULL(addon_index_fk, '0') = '0' AND ISNULL(Group_Dish, '0') = '0' THEN addon_index 
        WHEN ISNULL(addon_index_fk, '0') != '0' THEN addon_index_fk 
        ELSE Group_Dish 
    END AS item_index,
    MAX(Rate) AS Rate,
    SUM(Amount) AS Amount
INTO #RateAmountByItemIndex
FROM tbl_Bill_Tran
WHERE Bill_No_FK = @BillNo
GROUP BY 
    CASE 
        WHEN ISNULL(addon_index_fk, '0') = '0' AND ISNULL(Group_Dish, '0') = '0' THEN addon_index 
        WHEN ISNULL(addon_index_fk, '0') != '0' THEN addon_index_fk 
        ELSE Group_Dish 
    END;
-- Step 4: Insert Part B (grouped assorted items by qty)
-- Step 4: Insert Part B (grouped assorted items by qty, fixed with ROW_NUMBER subquery)
INSERT INTO #FinalResult
SELECT 
    '. ' + I_Name AS I_Name,
    Qty,
    CASE WHEN I_Name NOT LIKE '%. Done%' THEN Rate ELSE 0 END AS Rate,
    CASE WHEN I_Name NOT LIKE '%. Done%' THEN Amount ELSE 0 END AS Amount,
    '' AS dishComments,
    CAST(item_index AS VARCHAR),
    '' AS Step_index,
    -1 AS sno,
    1 AS SourceOrder
FROM (
    SELECT 
        g.I_Name,
        g.Qty,
        g.item_index,
        ISNULL(r.Rate, 0) AS Rate,
        ISNULL(r.Amount, 0) AS Amount,
        ROW_NUMBER() OVER (PARTITION BY g.item_index ORDER BY g.Qty) AS rn
    FROM #GroupedAssortedItems g
    LEFT JOIN #RateAmountByItemIndex r ON r.item_index = g.item_index
) AS t
-- Step 5: Final Output
SELECT * FROM #FinalResult 
WHERE Qty <> -1 AND I_Name NOT LIKE '%. Done%' 
ORDER BY item_index, SourceOrder DESC;
-- Cleanup
DROP TABLE #FinalResult;
DROP TABLE #GroupedAssortedItems;
DROP TABLE #RateAmountByItemIndex;

select *from TBL_CustomerInfo where id=@Cust_Code                 
--select distinct 'Vat@'+cast(a.tax as varchar)+'% : ' tax,Isnull(sum(b.Tax_Amount),0) 'Tax_Amount',bill_no_fk                                  
--from tbl_dish_mast a                              
--inner join tbl_bill_tran b                            
--on a.i_code=b.item_code where bill_no_fk=@BillNo                             
--group by a.tax,bill_no_fk                   
    EXEC Usp_GetAllTaxCrystal_Utility @BillNo=@BillNo         
-- discount              
select distinct 'Discount on '+cast(c.cat_name as varchar)+' @' Category,isnull(sum(a.dis_amount),0) as Dis_Amount,a.bill_no_fk from tbl_bill_tran a              
inner join tbl_dish_mast b on a.item_code=b.i_code              
inner join tbl_category_dish_mast c on b.cat_id=c.id              
where a.bill_no_fk=@BillNo and a.dis_amount>0              
group by c.cat_name,a.bill_no_fk   
Go



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_GetAllTaxCrystal_Utility]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Usp_GetAllTaxCrystal_Utility] 
Go
CREATE Proc [dbo].[Usp_GetAllTaxCrystal_Utility]-- 15769             
(              
@BillNo bigint              
)              
as              
begin              
--===============================================              
----Tax Cap----------------              
declare @ServiceTaxCap varchar(10)              
declare @SurchargeCap varchar(10)              
declare @SBCCap varchar(10)            
declare @KKCCap varchar(10)             
declare @DeliveryCap varchar(10)            
----Tax Amount Variable----------------              
declare @Dis_Amount money              
declare @Servicetax_Amount money              
declare @Surcharge money              
declare @SBC_Amount money            
declare @KKC_Amount money               
declare @Delivery_charge money              
declare @Roundoff money              
--======================create temp table==========================              
create table #tbl_temp(tax varchar(max),Tax_Amount money,bill_no_fk bigint,SLno int)              
--set value              
select @Dis_Amount=isnull(dis_amount,0),@Servicetax_Amount=Service_Tax,@Surcharge=sur_charge,@SBC_Amount=SBC_Tax,            
@Roundoff=RoundOff,@Delivery_charge=isnull(Other_Charge,0),@KKC_Amount=isnull(KKC_Tax,0) from tbl_bill where bill_no=@BillNo              
--set tax cap              
select @ServiceTaxCap=Service_tax,@SurchargeCap=Sur_charge,@SBCCap=SBC_Tax,@KKCCap=KKC_Tax,@DeliveryCap=DeliveryCharge from tbl_config_mast              
-- discount                      
if(@Dis_Amount>0)              
insert into #tbl_temp(tax,Tax_Amount,bill_no_fk,SLno)values('Discount @ :',@Dis_Amount,@BillNo,0)              
--del charge      
if(@Delivery_charge>0)            
insert into #tbl_temp(tax,Tax_Amount,bill_no_fk,SLno)values('Pkg/del Charge@ :',@Delivery_charge,@BillNo,1)                 
-- VAT              
--insert into #tbl_temp              
--select distinct 'Vat@'+cast(a.tax as varchar)+'% : ' tax,Isnull(sum(b.Tax_Amount),0) 'Tax_Amount',bill_no_fk                                            
--from tbl_dish_mast a                                        
--inner join tbl_bill_tran b                                      
--on a.i_code=b.item_code where bill_no_fk=@BillNo  and b.Tax_Amount>0                                      
--group by a.tax,bill_no_fk  
  insert into #tbl_temp(tax,Tax_Amount,bill_no_fk,SLno)
              SELECT  a.Tax_display_name+' :' as tax,sum(a.Tax_Amt) as tax_amount,a.bill_no_fk,3 as SLno FROM tbl_bill_tax a  
   WHERE a.Bill_No_FK  =@BillNo and Tax_Amt>0  
  group by Tax_display_name,a.bill_no_fk  
---- Service_Tax                      
--if(@Servicetax_Amount>0)              
--insert into #tbl_temp(tax,Tax_Amount,bill_no_fk)values('Service Tax@'+@ServiceTaxCap+'% :',@Servicetax_Amount,@BillNo)              
----SBC Tax              
--if(@SBC_Amount>0)              
--insert into #tbl_temp(tax,Tax_Amount,bill_no_fk)values('SBC@'+@SBCCap+'% :',@SBC_Amount,@BillNo)             
----KKCC Tax              
--if(@KKC_Amount>0)              
--insert into #tbl_temp(tax,Tax_Amount,bill_no_fk)values('KKC@'+@KKCCap+'% :',@KKC_Amount,@BillNo)             
---- Surcharge                    
--if(@Surcharge>0)              
--insert into #tbl_temp(tax,Tax_Amount,bill_no_fk)values('Surcharge@'+@SurchargeCap+'% on vat :',@Surcharge,@BillNo)              
--round off             
insert into #tbl_temp(tax,Tax_Amount,bill_no_fk,SLno)values('       R. Off :',@Roundoff,@BillNo,10)              
---select               
select *from #tbl_temp              
drop table #tbl_temp              
end
Go







IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_Get_Printer_by_Counter_MP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Usp_Get_Printer_by_Counter_MP] 
Go
Create proc [dbo].[Usp_Get_Printer_by_Counter_MP]  --@Bill_No_FK='16610'         
(                                       
@Bill_No_FK varchar(50)                                       
)                                       
as                                       
begin                
declare @Channel varchar(100),@Channel_Order_Id varchar(100),@TokenNo varchar(100) 
select @Channel=Channel,@Channel_Order_Id=Channel_Order_Id,@TokenNo=TockenNo from tbl_bill where bill_no=@Bill_No_FK                  
select distinct c.c_code,isnull(c.printer,0) as printer,                  
c.C_Name as 'Counter',@Channel as 'Channel',@Channel_Order_Id as 'Channel_Order_Id',@TokenNo as 'TokenNo'  
 from tbl_bill_tran a  WITH (NOLOCK) inner join                              
tbl_dish_mast b on a.item_code=b.i_code                              
inner join tbl_Counter_Mast c on b.counter_id=c.c_code                             
where a.Bill_no_fk=@Bill_No_FK                              
-- combo-----------------          
select a.id,a.i_code,'* '+substring(lower(b.i_name),1,33)'i_name',a.i_code_fk,'1' as qty,                  
a.bill_no,item_index,Step_index,isnull(b.kot_flag,0) as 'kot_flag',a.DishComment      
 from tbl_assorted_item a   WITH (NOLOCK)                                 
inner join tbl_dish_mast b on a.i_code=b.i_code where a.bill_no=@Bill_No_FK  and upper(b.i_name)<>'NA'      
 and b.i_name not like'%WHITE BUN%'               
order by a.Step_index,b.i_name,a.DishComment           
-- addon-----------------          
select a.id,substring('+ '+a.i_name,1,35)'i_name',item_code,qty,convert(varchar,amount,1) as amount,                                    
(case (Comments) when 'null' then '' when null then '' else Comments end) Comments,                        
(case(item_addon) when ' ' then '0' else item_addon end) item_addon,isnull(Group_Dish,0) Group_Dish,            
isnull(Urgent_Item,0) as 'Urgent_Item',addon_index_fk as 'addon_index',isnull(a.kot_flag,0) as 'kot_flag'                
from tbl_bill_tran   WITH (NOLOCK)                                     
inner join tbl_dish_mast a on item_code=a.i_code and bill_no_fk=@Bill_No_FK            
 and upper(a.i_name)<>'NA' and isnull(addon_index_fk,'0')!='0'          
order by tbl_bill_tran.id          
end  
Go




IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].Usp_Get_Item_KOT_Counter_MP') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].Usp_Get_Item_KOT_Counter_MP 
Go
CREATE proc [dbo].Usp_Get_Item_KOT_Counter_MP --@Bill_No_FK='16590',@counter_id='1'                           
(                                  
@Bill_No_FK varchar(50),                               
@counter_id bigint                  
)                                  
as                                  
begin                                  
select a.id,  
--substring(a.i_name,1,35)'i_name',  
 CASE 
        WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No=@Bill_No_FK AND i_code_fk=a.I_Code)
             AND EXISTS (SELECT 1 FROM tbl_dish_Step_mast WHERE i_code_fk=a.I_Code) THEN ''
        --WHEN EXISTS (SELECT 1 FROM tbl_assorted_item WHERE Bill_No=@BillNo AND i_code_fk=b.I_Code) THEN ''
        ELSE
        substring(a.i_name,1,35)
  END AS 'i_name',
item_code,qty,convert(varchar,amount,1) as amount,                                
(case (bt.Comments) when 'null' then '' when null then '' else bt.Comments end) Comments,                    
(case(item_addon) when ' ' then '0' else item_addon end) item_addon,isnull(Group_Dish,0) Group_Dish,        
isnull(Urgent_Item,0) as 'Urgent_Item',addon_index,isnull(a.kot_flag,0) as 'kot_flag'   
,a.POS_Desc as I_Name_Desc,  
(case when Channel_Order_Id like '%-%' then   
 ( left(Channel_Order_Id, charindex('-', Channel_Order_Id) - 1))  
else   
Channel_Order_Id end)  
 as zomato_order_id           
from tbl_bill_tran bt WITH (NOLOCK) 
inner join tbl_bill bm with (nolock) on  bt.Bill_No_FK=bm.Bill_No                                   
inner join tbl_dish_mast a on item_code=a.i_code and bill_no_fk=@Bill_No_FK           
and a.counter_id=@counter_id    and upper(a.i_name) <>'NA' and isnull(addon_index_fk,'0')='0'      
order by bt.id          
end  
Go




IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_GetCompany_Mast_OrderPrint]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Usp_GetCompany_Mast_OrderPrint] 
Go
Create   proc [dbo].[Usp_GetCompany_Mast_OrderPrint]                           
as                   
begin    
select top 1 Company_code as Outlet_id,Company_Name,(Address+'-'+Pin_Code) as 'address',                          
Tin_No as Tin_No,STax_No as STax_No,'TEL:'+Phone_No as Phone_No,Keys,convert(varchar(10),Validity,120) as 'Validity',Footer1,Footer2,Footer3,                  
ServiceCharge,cin_no,fssai_no,                
(select top 1 convert(varchar(10),bill_date,120)as 'DayEnd'      
 from tbl_day_end  with (nolock)where isdayend=0 order by bill_date desc) as 'Bill_date',Outlet_Name,                
isnull((select top 1 MailHost from tbl_mail_Config with (nolock)),'N/A') as MailHost,                
isnull((select top 1 Password from tbl_mail_Config with (nolock)),'N/A') as Password,            
isnull((select top 1 FromMail from tbl_mail_Config with (nolock)),'N/A') as FromMail,sqlkey_erp,sqlkey_primary ,sqlkey_erp as sqlkey_CloudDB               
from tbl_company_master with (nolock)        
select Outlet_id,cast(Service_tax as float) Service_tax,cast(Service_charge as float)    
 Service_charge,cast(Sur_charge as float) Sur_charge,                                                                              
cast(Service_Charge_HD as float) as Service_Charge_HD,sqlKey,Coupon_Item,POSCode,Print_Cap,                                        
PortName,Kot_Cut,NewBill,Generate_File,TenantID,POS_NO,ShiftNo,IsTraining,TRAN_STATUS,                                                                              
host_log,User_log,pass_log,AM_Active,header,SendMail,LockSystem,EXE_Constants,IsKot,No_of_Kot,    
Print_Size,Client_ID,DeliveryCharge,Log_Drive,                                                                        
CRM_Open_Cap,CRM_Mad_Cap,Dual_Display ,Dual ,IsPrintSettlement ,TA_Stax ,DI_Stax ,HD_Stax ,TA_Sur_CH     
,DI_Sur_CH ,HD_Sur_CH  ,                                                                       
Print_No_of_HD_Bill  ,KDS_Active,Day_End_Validation,Validation_Date,IsShowStockOut,round(isnull(SBC_Tax,0),2) as SBC_Tax,validate_ip,IsHdPC,TaxCap,                    
SoftwareTitle,loginExe,MainFromTitle,Genrate_File,DiscountCap ,Noof_bill_TA,Print_No_of_CB_Bill,GRN_Cap,Dual_Display,IsTabOrder,                                          
convert(varchar,isnull(SBC_Tax,0),0) as 'SBC_Tax',convert(varchar,isnull(KKC_Tax,0),0) as 'KKC_Tax',QtyPriceAsk,Settlement_optin_HD,                                      
isnull(NoofPrint_KOT_TA,0) as 'NoofPrint_KOT_TA',isnull(NoofPrint_KOT_DN,0) as 'NoofPrint_KOT_DN',                                      
isnull(NoofPrint_KOT_HD,0) as NoofPrint_KOT_HD,isKOT_3or4_EnchPrint,CRMCap_TA,CRMCap_DN,SMS_HD,SMS_TA,SMS_DN,                          
isnull(IsZomato,0) as IsZomato,Cutkot_UrgentItem,Start_Sub_Dept,ShowTax_Dual_display,                  
isnull(IsItemSeperate,0    
) as 'IsItemSeperate',Isnull(GST_on_delivery,0) as 'GST_on_delivery',                  
isnull(DelCharge_type,'') as 'DelCharge_type',Report_validation,IsNotifyOrder,SpeechText,Bill_Head,Print_hsn,          
PrintPoweredby,StockOutTime,IsFill_Finishitem_DSC,        
(select Aggr_Outlet_Id from tbl_ZomatoConfg  with (nolock) ) as Aggr_Outlet_Id,IsBillSend,IsClosingAmountSend,IsShiftCValidation,    
Weekoff_Validation,IsPrintRemarksOnKotHD,IsopenCardSelect_inHDSettle,PrintComboRate,    
0 as 'Card_Expire_Days',0 as 'Security_D    
eposit',SingleKOT,pkg_charge_dn,pkg_charge_TA    
--isnull((select top 1 Card_Expire_Days from tbl_Recharge_Config_Mast with (nolock)),'1') as Card_Expire_Days,    
--isnull((select top 1 Security_Deposit from tbl_Recharge_Config_Mast with (nolock)),'1') as Security_Deposit    
,Ischangeinyear,Ischangeinyear_msg,fin_year,IsPrintBackDateBill,IsEDC_Live,menu_height,menu_width,prep_time_mins,    
isDSCTempSave,FoodOrdersNewPage,IsShowFullItemNameOnBill,0 as security_deposit ,isnull(Initial,'') as  Initial,  
(select OnlineUtility_version from tbl_version_update with(nolock) where isnull(IsValid,0)=1 ) as AvailableVersion , 
Isnull(IsCheckAsyncBillsOnEOD,0)  as IsCheckAsyncBillsOnEOD,Isnull(isPrintZomatoIdOnKotHD,0)  as isPrintZomatoIdOnKotHD
from tbl_Config_Mast  with (nolock)            
select Outlet_id,dsc_damage,    
void_KOT,void_bill,Nc_Bill,Order_rec_variance,ERP_login,Recipe_update,                                                            
Portion_update,Item_entry_update,Bulk_edit_update,Production_note,Production_input,Auditor_inventory_entry,created_by,created_on,                           
updated_by,updated_on,modify_bill,report,hold_bill,MailHost,Password,FromMail,food_order,PortNo from tbl_mail_Config  with (nolock)                                                               
 select Outlet_id,IsEasyRewards_Cap,ERAdminUserName,ERUserName,ERPassword,ERDEVID,ERAPPID,ERProgramCode,            
 ERStoreCode,ERActivityCode,Create_On,Created_By,Update_On,Update_By from tbl_Loyaltycard_Mast with (nolock)                
 select  IseWords,ApiKeyName,ApiKey,merchant_id,eWemail,sub_id,type,account_type,        
addPoint_Url,RedeemPoint_Url,OtpConfirm_Url,BalancePoint_Url,checkRewardPos_Url,Verify_coupon,Redeem_coupon    
 from tbl_CRM_Config   with (nolock)      
end
Go