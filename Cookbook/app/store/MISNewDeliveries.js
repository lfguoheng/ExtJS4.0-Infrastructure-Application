Ext.define('CookBook.store.MISNewDeliveries', {
    extend: 'Ext.data.Store',
    model: 'CookBook.model.MISNewDelivery',

    autoLoad: false,
    
    proxy: {
        type: 'ajax',

        actionMethods: {
            update: 'PUT',
            read: 'GET',
            destroy: 'DELETE',
            create: 'POST'
        },
        
        url: 'GetMISNewDelivery.ashx',
        limitParam: 'undefined',
        
        //noCache: false,
        reader: {
            type: 'json',
            root: 'rows',
            totalProperty: 'total',
            successProperty: 'success',
            id: 'mis_new_id'
        },

        afterRequest: function(request, success) {/*
            console.log(request.action);
            console.log(request.method);
            console.log(request.params);
            console.log(request.url);
            console.log('Succeeded? Actually ' + success);*/
        }
    }
});