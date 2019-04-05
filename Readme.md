<!-- default file list -->
*Files to look at*:

* **[XPOInstantModeCRUDBehavior.cs](./CS/CRUDBehavior.SL/XPOInstantModeCRUDBehavior.cs) (VB: [XPOInstantModeCRUDBehavior.vb](./VB/CRUDBehavior.SL/XPOInstantModeCRUDBehavior.vb))**
* [MainPage.xaml](./CS/XPOInstant.SL/MainPage.xaml) (VB: [MainPage.xaml](./VB/XPOInstant.SL/MainPage.xaml))
* [MainPage.xaml.cs](./CS/XPOInstant.SL/MainPage.xaml.cs) (VB: [MainPage.xaml.vb](./VB/XPOInstant.SL/MainPage.xaml.vb))
<!-- default file list end -->
# How to implement CRUD operations using DXGrid and eXpress Persistent Objects


<p><br />This example shows how to use XPInstantFeedbackSource or XPServerCollectionSource with DXGrid, and how to implement CRUD operations (e.g., add, remove, edit) in your application via special behavior.</p>
<p>The test sample requires the SQL Express service and MSAccess to be installed on your machine.</p>
<p>We have created the XPOServerModeCRUDBehavior and XPOInstantModeCRUDBehavior attached behaviors for GridControl. For instance:</p>
<pre class="cr-code">[XML]<code><dxg:GridControl>

   <i:Interaction.Behaviors>

       <crud:XPOServerModeCRUDBehavior .../>

   </i:Interaction.Behaviors>

</dxg:GridControl></code></pre>
<p> </p>
<p>The XPServerModeCRUDBehavior and XPInstantModeCRUDBehavior classes contain the NewRowForm and EditRowForm properties to provide the "Add Row" and "Edit Row" actions. With these properties, you can create the Add and Edit forms according to your requirements:</p>
<pre class="cr-code">[XML]<code><DataTemplate x:Key="EditRecordTemplate">

   <StackPanel Margin="8" MinWidth="200">

       <Grid>

           <Grid.ColumnDefinitions>

               <ColumnDefinition/>

               <ColumnDefinition/>

           </Grid.ColumnDefinitions>

           <Grid.RowDefinitions>

               <RowDefinition/>

               <RowDefinition/>

           </Grid.RowDefinitions>

           <TextBlock Text="ID:" VerticalAlignment="Center" Grid.Row="0" Grid.Column="0" Margin="0,0,6,4" />

           <dxe:TextEdit x:Name="txtID" Grid.Row="0" Grid.Column="1" EditValue="{Binding Path=Id, Mode=TwoWay}" Margin="0,0,0,4" />

           <TextBlock Text="Name:" VerticalAlignment="Center" Grid.Row="1" Grid.Column="0" Margin="0,0,6,4" />

           <dxe:TextEdit x:Name="txtCompany" Grid.Row="1" Grid.Column="1" EditValue="{Binding Path=Name, Mode=TwoWay}" Margin="0,0,0,4" />

       </Grid>

   </StackPanel>

</DataTemplate>

<crud:XPServerModeCRUDBehavior NewRowForm="{StaticResource ResourceKey=EditRecordTemplate}" EditRowForm="{StaticResource ResourceKey=EditRecordTemplate}"/></code></pre>
<p>This Behavior classes requires the following information from your data model:</p>
<p>- XPObjectType - the type of rows;</p>
<p>- DataServiceContext - database entities;</p>
<p>- PrimaryKey - the primary key of the database table;</p>
<p>- CollectionSource/InstantCollectionSource - an object of the EntityServerModeDataSource type.</p>
<pre class="cr-code">[XML]<code><dxg:GridControl>

   <i:Interaction.Behaviors>

       <crud:XPOServerModeCRUDBehavior XPObjectType="{x:Type local:Items}" CollectionSource="{Binding Collection}" PrimaryKey="Id"/>

   </i:Interaction.Behaviors>

</dxg:GridControl></code></pre>
<p>The XPInstantModeCRUDBehavior class for SL contains the ServiceHelper property, which refers to an object that provides actions to work with databases</p>
<pre class="cr-code">[C#]<code>helper.ServiceHelper = new ServiceHelper(helper, new Uri("<a href='http://localhost'>http://localhost</a>:54177/WcfDataService.svc/"));</code></pre>
See the <a href="http://documentation.devexpress.com/#XPO/clsDevExpressXpoXPInstantFeedbackSourcetopic"><u>XPInstantFeedbackSource</u></a> and <a href="http://documentation.devexpress.com/#XPO/clsDevExpressXpoXPServerCollectionSourcetopic"><u>XPServerCollectionSource</u></a> classes to learn more about XPInstantFeedbackSource and XPServerCollectionSource.
<p> </p>
<p>Behavior class descendants support the following commands: NewRowCommand, RemoveRowCommand, EditRowCommand. You can bind your interaction controls with these commands with ease. For instance:</p>
<pre class="cr-code">[XML]<code><crud:XPOServerModeCRUDBehavior x:Name="helper"/>

<StackPanel Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Center">

   <Button Height="22" Width="60" Command="{Binding Path=NewRowCommand, ElementName=helper}">Add</Button>

   <Button Height="22" Width="60" Command="{Binding Path=RemoveRowCommand, ElementName=helper}" Margin="6,0,6,0">Remove</Button>

   <Button Height="22" Width="60" Command="{Binding Path=EditRowCommand, ElementName=helper}">Edit</Button>

</StackPanel></code></pre>
<p>By default, the XPOServerModeCRUDBehavior and XPOInstantModeCRUDBehavior solution support the following end-user interaction capabilities:</p>
<p>1. An end-user can edit selected row values by double-clicking on a grid row or by pressing the Enter key if the AllowKeyDownActions property is True.</p>
<p>2. An end-user can remove selected rows via the Delete key press if the AllowKeyDownActions property is True.</p>
<p>3. An end-user can add new rows, remove and edit them via the NewRowCommand, RemoveRowCommand, and EditRowCommand commands.<br /><br /><br />To learn more on how to implement similar functionality in <strong>WPF,</strong> refer to the <a href="https://www.devexpress.com/Support/Center/p/E3868">E3868</a> example.</p>

<br/>


