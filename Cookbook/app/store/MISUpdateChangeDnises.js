Ext.define('CookBook.store.MISUpdateChangeDnises', {
	extend: 'Ext.data.Store',
	model: 'CookBook.model.MISUpdateChangeDnis',

	autoLoad: false,
	
	proxy: {
		type: 'ajax',

		actionMethods: {
			update: 'PUT',
			read: 'GET',
			destroy: 'DELETE',
			create: 'POST'
		},
		
		url: 'UpdateMISUpdateChangeDnises.ashx',
		limitParam: 'undefined',
		
		//noCache: false,
		
		reader: {
			type: 'json',
			root: 'rows',
			totalProperty: 'total',
			successProperty: 'success',
			idProperty: 'mis_updatednis_id'
		},

		writer: {
			type: 'json',
			root: 'rows',
			totalProperty: 'total',
			successProperty: 'success',
			idProperty: 'mis_updatednis_id'
		}
	},
	
	listeners: {
		datachanged: function (store, opts) {
			store.sync();
		},
		update: function (store, record, operation, opts) {
			store.sync();
		}
	}
});