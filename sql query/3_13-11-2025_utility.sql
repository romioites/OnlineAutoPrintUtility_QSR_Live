alter   proc [dbo].Usp_PendingOrderList_CURD  
(  
@bill_date varchar(20),  
@mode int=0,  
@bill_no bigint=null,  
@fin_year varchar(20)=null,  
@online_bill_no bigint=null,  
@Current_Status varchar(20)=null,  
@Status_Website_api int=0,  
@Status_Cloud_DB tinyint=0,  
@zomato_order_id  varchar(30)=null  
)  
as    
begin   
  
  
if(@mode=0)  
begin  
  
select @bill_date=bill_Date from tbl_day_end where isdayend=0  
  
insert tbl_order_status_update(bill_no,fin_year,Status_Website_api,Status_Cloud_DB,online_bill_no,Current_Status,bill_date,CreatedOn,zomato_order_id)  
values(@bill_no,@fin_year,@Status_Website_api,@Status_Cloud_DB,@online_bill_no,@Current_Status,@bill_date,GETDATE(),@zomato_order_id)  
end  
else if(@mode=1)  
begin  
--select id,bill_no,fin_year,isnull(Status_Website_api,0) as Status_Website_api,online_bill_no,  
--isnull(Status_Cloud_DB,0) as Status_Cloud_DB,Current_Status,zomato_order_id from  
--tbl_order_status_update with(nolock) where convert(varchar(10),bill_date,120)=@bill_date  
--and ( isnull(Status_Cloud_DB,0)=0 or isnull(Status_Website_api,0)=0 )  order by id   
  
select *from View_ListOfPndingStatus where convert(varchar(10),bill_date,120)=@bill_date  
and ( isnull(Status_Cloud_DB,0)=0 or isnull(Status_Website_api,0)=0 )  
and RetryCount<=4  
  
--union all  
--select b.id,a.bill_no,a.fin_year,isnull(a.is_api_sync,0) as Status_Website_api,0 as'online_bill_no','1' as Status_Cloud_DB, 'CustomerDataSent' as 'Current_Status',  
  
  
--'' zomato_order_id,a.bill_date,0 as RetryCount  
--from TBL_CustomerInfo b   
--inner join tbl_Bill a with(nolock) on b.id=a.cust_code where convert(varchar(10),a.bill_date,120)=@bill_date  
--and isnull(a.is_api_sync,0)=0 and   a.Channel not in('swiggy','zomato')  
--order by id   
  
end  
else if(@mode=2)  
begin  
  
update tbl_order_status_update set Status_Website_api=@Status_Website_api where bill_no=@bill_no and Current_Status=@Current_Status  
  
end  
else if(@mode=3)  
begin  
update tbl_order_status_update set Status_Cloud_DB=@Status_Cloud_DB where bill_no=@bill_no and Current_Status=@Current_Status  
end  
else if(@mode=4)  
begin  
update tbl_order_status_update set RetryCount=isnull(RetryCount,0)+1 where bill_no=@bill_no and Current_Status=@Current_Status  
end  
else if(@mode=5)  
begin  
update tbl_bill set is_api_sync=1  where bill_no=@bill_no  
end  
end

go

declare @bill_date varchar(20)
select @bill_date=bill_Date from tbl_day_end where isdayend=0  
update tbl_order_status_update set RetryCount=0 where bill_date=@bill_date
go



alter Procedure [dbo].[USP_UrlConfig] --@SourceId='7',@actionType='delivered'    
@SourceId int = null,    
@actionType varchar(50) = null    
as    
Begin    
if(@SourceId is not null and @SourceId!='' and @SourceId!='0')  
begin  
 Select url,ApiKeyName,ApiKey,actionType,SourceName from [dbo].tbl_ZomatoUrlConfg where SourceId=@SourceId and actionType=@actionType    
 end  
 else if(@SourceId=0 and @actionType='')  
begin  
 Select url,ApiKeyName,ApiKey,actionType,SourceName from [dbo].tbl_ZomatoUrlConfg --where SourceId=@SourceId and actionType=@actionType    
 end  
 else  
 begin  
 Select url,ApiKeyName,ApiKey,actionType,SourceName from [dbo].tbl_ZomatoUrlConfg where actionType=@actionType  
  and SourceName='Urban Piper'  
 end  
End    
go