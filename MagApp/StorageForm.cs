﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MagApp
{
    public partial class StorageForm : Form
    {

        Product currentprod;
        float total;
        public StorageForm()
        {
            InitializeComponent( );
            total = 0.0f;
            currentprod = new Product( );
            labltotal.Text = "0.00 MAD";
            foreach( Product prod in Product.List )
                combproducts.Items.Add( prod.Lable );

            combproducts.Text = combproducts.Items[ 0 ].ToString( );

            // HERE: show the list of in product of today and add a button 
            // to swap between the past and previous ins 
            dgvstorage.DataSource = currentprod.Storage.In;
        }

        private void btnaddtolist_Click( object sender, EventArgs e )
        {
            // TODO: fix the add in the list
            //


            string newstring = "";

            /* WHAT: whether the product is already in the list */
            bool exists = false;
            int index;

            for( index = 0; index < listadded.Items.Count; ++index ) {
                string lable;
                decimal newvalue = 0;

                #region Setup
                string[ ] str = ( ( string ) listadded.Items[ index ] ).Split( new char[ 2 ] { '(', ')' } );
                lable = str[ 0 ].TrimEnd( );
                newvalue = 0;
                #endregion

                bool isit = string.Equals( currentprod.Lable, lable );

                if( isit ) {
                    // add the new and the previous values
                    newvalue = ( numquantity.Value + int.Parse( str[ 1 ] ) );
                    // update the list item overview
                    newstring = FormatLabel( currentprod.Lable, ( numquantity.Value = newvalue ) );
                    // flag it's existance
                    exists = true;
                    break;
                }

            }

            if( !( exists ) ) {
                // setup a fresh item
                listadded.Items.Add( FormatLabel( currentprod.Lable, numquantity.Value ) );
                // select the item
                listadded.SetSelected( listadded.Items.Count - 1, true );
            } else {
                // it exists!
                // you may want to check this `i`
                listadded.Items[ index ] = newstring;
                listadded.SetSelected( index, true );
            }

            if( listadded.SelectedItem != null ) {
                string[ ] info = listadded.SelectedItem.ToString( ).Split( new char[ 2 ] { '(', ')' } );

                combproducts.Text = info[ 0 ].TrimEnd( );
                numquantity.Value = int.Parse( info[ 1 ] );


                // TODO: find how to take few digits from 
                // the foalt number
                // 

                foreach( string item in listadded.Items ) {
                    string[ ] str = item.ToString( ).Split( new char[ 2 ] { '(', ')' } );

                    if( string.Equals( str[ 0 ].TrimEnd( ), currentprod.Lable ) ) {
                        total += ( float.Parse( str[ 1 ] ) * currentprod.Price );
                        break;
                    }
                }

                labltotal.Text = string.Format( "{0:0.00} MAD", total.ToString( ) );
            }

        }

        private string FormatLabel( string lable, decimal quantity )
        {
            return string.Format( "{0} ({1})", lable, quantity );
        }

        private void btnremove_Click( object sender, EventArgs e )
        {
            if( listadded.SelectedItem != null ) {
                int index = listadded.SelectedIndex;
                string[ ] str = listadded.Items[ index ].ToString( ).Split( new char[ ] { '(', ')' } );

                // the total taht is stored in teh label
                float price = 0.0f;

                foreach( Product prod in Product.List )
                    if( string.Equals( prod.Lable, str[ 0 ].TrimEnd() ) ) {
                        price = prod.Price;
                        break;
                    }

                // minus teh price of the removed items
                total -= ( float.Parse( str[ 1 ] ) * price );

                labltotal.Text = string.Format("{0} MAD",total.ToString( ));
                listadded.Items.Remove( listadded.Items[ index ] );
            } else { labnotif.Text = "Select a product first"; }
        }

        private void btnconfirm_Click( object sender, EventArgs e )
        {
            // TODO: bind the datagrid and update the XML file
            //

            // get the current list of incomming storage
            List<Product> current = new List<Product>( );

            foreach( string item in listadded.Items ) {
                string[ ] str = item.Split( new char[ ] { '(', ')' } );
                // List all the products
                foreach( Product prod in Product.List )
                    if( prod.Lable == str[ 0 ].TrimEnd( ) ) {
                        current.Add( prod );
                        prod.Storage.IncomingStorage( prod, int.Parse( str[ 1 ].TrimEnd( ) ) );
                    }
            }

            // bind the datagridview
            dgvstorage.DataSource = current.ToList( );



        }

        private void listadded_SelectedIndexChanged( object sender, EventArgs e )
        {
            labnotif.Text = "";
            if( listadded.SelectedItem != null ) {
                string[ ] str = listadded.SelectedItem.ToString( ).Split( new char[ ] { '(', ')' } );
                combproducts.Text = str[ 0 ].TrimEnd( );
                numquantity.Value = decimal.Parse( str[ 1 ] );
            }
        }

        private void combproducts_SelectedIndexChanged( object sender, EventArgs e )
        {
            // TODO: update the quantity lable
            // done.

            string labcurrent = combproducts.Text;

            foreach( Product prod in Product.List )
                if( prod.Lable == labcurrent && currentprod.Lable != labcurrent ) {
                    // get the prod
                    prod.CopyTo( currentprod );
                    lablquant.Text = string.Format( "({0})", currentprod.Storage.Quantity );

                    for( int index = 0; index < listadded.Items.Count; ++index ) {
                        string[ ] str = ( ( string ) listadded.Items[ index ] ).Split( new char[ ] { '(', ')' } );
                        bool isit = ( str[ 0 ].TrimEnd( ) == labcurrent ) ? true : false;

                        listadded.SetSelected( index, isit );

                        if( isit ) {
                            numquantity.Value = int.Parse( str[ 1 ] );
                            /* you need a */
                            break;
                            /* you need KitKat */
                        }
                        // rempplace all the foreach-loops witha  standard forloop
                    }
                    break; // we found it!
                }
        }

        private void combproducts_TextUpdate( object sender, EventArgs e )
        {


            string labcurrent = combproducts.SelectedItem.ToString( );

            foreach( Product prod in Product.List )
                if( prod.Lable == labcurrent ) { prod.CopyTo( currentprod ); break; }

            lablquant.Text = string.Format( "({0})", currentprod.Storage.Quantity );
        }
    }
}

