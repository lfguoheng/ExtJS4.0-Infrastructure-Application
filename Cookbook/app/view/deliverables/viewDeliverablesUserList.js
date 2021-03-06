Ext.define('CookBook.view.deliverables.ViewDeliverablesUserList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.viewDeliverablesUserList',
    
    name: 'deliverablesUserList',
    
    title: 'Your Personal Deliverables',
    isFiltered: false,
    store: 'Deliverables',
    columns: [{
        xtype: 'actioncolumn',
        width: 22,
        items: [{
            icon: 'extjs/examples/shared/icons/fam/add.png',
            tooltip: 'click to add this assumption to the project',
            handler: function(grid, rowIndex, colIndex){
                var deliverable = grid.getStore().getAt(rowIndex).get('deliverable_text');
                //var category = grid.getStore().getAt(rowIndex).get('category');
                
                GLOBAL_currentController.getStore("ProjectDeliverables").add({
                    project_id: GLOBAL_currentProjectOpenProjectID,
                    deliverable_text: deliverable
                }); //, category: category});
            }
        }]
    }, {
        header: 'Owner',
        width: 144,
        dataIndex: 'user_name'
    }, //can't edit user names
    {
        header: 'Deliverable',
        width: 170,
        flex: 1,
        dataIndex: 'deliverable_text',
        editor: {
            xtype: 'textarea'
        }
    },    /*
     { header: 'Category', width: 144, dataIndex: 'category', editor: {
     xtype: 'combobox',
     store: 'Categories',
     multiSelect:		false,
     displayField:		'category1',
     valueField:			'category1',
     allowBlank:			true,
     matchFieldWidth:	true,
     listConfig: {
     autoHeight: true
     },
     queryMode:			'local'
     }},*/
    {
        header: 'BusinessUnit',
        width: 144,
        dataIndex: 'business_unit',
        editor: {
            xtype: 'combobox',
            store: 'BusinessUnits',
            multiSelect: false,
            displayField: 'name',
            valueField: 'name',
            allowBlank: true,
            matchFieldWidth: true,
            listConfig: {
                autoHeight: true,
                loadMask: false
            },
            queryMode: 'local'
        }
    }, {
        xtype: 'actioncolumn',
        width: 22,
        items: [{
            icon: 'extjs/examples/restful/images/delete.png',
            tooltip: 'click to delete this row',
            handler: function(grid, rowIndex, colIndex){
                if (grid.getStore().getAt(rowIndex).get('user_name').toLowerCase() != GLOBAL_username.toLowerCase()) {
                    return;
                }
                grid.getStore().removeAt(rowIndex);
            }
        }]
    }],
    selType: 'cellmodel',
    columnLines: true,
    plugins: [Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 2,
        listeners: {
            beforeedit: function(e){
                if (e.record.get('user_name').toLowerCase() != GLOBAL_username.toLowerCase()) {
                    return false;
                }
            }
        }
    })],
    viewConfig: { //stripeRows: true
}    ,
    tools: [{
        type: 'plus',
        tooltip: 'Add another entry to this table',
        handler: function(event, toolEl, panel){
            var gridPanel = panel.up();
            gridPanel.getStore().add({
                user_name: GLOBAL_username
            }); //add an empty row
        }
    }, {
        type: 'search',
        tooltip: 'Click to filter your personal deliverables',
        handler: function(event, toolEl, panel){
            var gridPanel = panel.up();
            if (gridPanel.isFiltered == true) {
                gridPanel.getStore().clearFilter();
                gridPanel.isFiltered = false;
            }
            else {
                gridPanel.isFiltered = true;
                var regstr = new RegExp(GLOBAL_username.toLowerCase(), 'i')
                gridPanel.getStore().filter({
                    id: 'user_name',
                    property: 'user_name',
                    value: regstr
                });
            }
        }
    }]
});
