   M S B A M L   `   `   `  ����C  ?Missions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Missions.ViewsMissions.Views  5   	       Missions.Views.BoardWin   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:Missions.Views     5   	     � Title$)  #Доска пользователя��5   	   $
��1080��6$   $
��1920��62    � WindowState$ 	Maximized=�6?    � 
ResizeMode$ NoResize=�6W   ���	#FF8CD2CD � 	Resources 5	         MyCustomFont �� Property �� Value��%Z   &	        �� 5
   
   .�5      ��  � ��$ ../Fonts/#Montserrat)�67   5      5      .��5      ]� .+�5   
   �� $	��Top=�6    �� Orientation$ 
Horizontal=�6*   $
��Left=�6C   .+�5      �� -   5         ��Name$ 
exitButton��5   W   ���	#FF8CD2CD$#��../Fonts/#Montserrat SemiBold)�6,   $��30��6k   $	��103��6w   $��16(�6�   $��
10 0 700 0q�6�   .��6�   
Выйти6�   �� 5      
���   �$��MISSIONS��6+   $#��../Fonts/#Montserrat SemiBold)�6;   $��55(�6f   $��6 7 0 0q�6t   �� 5      ���	#FFFFBE49$��MISSIONS��6/   $#��../Fonts/#Montserrat SemiBold)�6?   $��55(�6j   $��
-285,5,0,0q�6x   5      �� 5   
   $	��Top=�6   $ 
Horizontal=�6*   $��Center=�6C   $��	0,10,0,10q�6`   .+�5      �� -   5        $ personalButton��5   �   ���	#FFFFBE49$#��../Fonts/#Montserrat SemiBold)�6,   $��25(�6W   $!��Личный кабинет��6e   $	��235��6~   
���   �$��35��6�   $��0,0,10,0q�6�   6�   �� 5      -   5        ���	#FFFFBE49$#��../Fonts/#Montserrat SemiBold)�5   ,   $��25(�6W   $!��Создать проект��6e   $	��235��6�   $��35��6�   
���   �6�   5      �� 5   
   $
��Auto=�6   $��Bottom=�6:   .��5      � -   5         �Name$ ProjectsPanel��5   ?   #���   6   	 � Orientation$	 
Horizontal=�6V   $��0,0,0,0q�6o   5      5      5      