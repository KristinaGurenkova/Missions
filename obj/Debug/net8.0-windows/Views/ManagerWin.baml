   M S B A M L   `   `   `  ����C  ?Missions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Missions.ViewsMissions.Views  5   	       Missions.Views.ManagerWin   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:Missions.Views     5   	     � Title$!  Окно менеджера��5   	   $	��760��6    $	��950��6-    � 
ResizeMode$ NoResize=�69   ���	#FF8CD2CD � 	Resources 5	         MyCustomFont �� Property �� Value��%Z   &	        �� 5
   
   .�5      ��  � ��$ ../Fonts/#Montserrat)�67   5      5      .��5      ]� .+�5   
   �� $	��Top=�6    �� Orientation$ 
Horizontal=�6*   $
��Left=�6C   .+�5      �� -   5         ��Name$ 
exitButton��5   W   ���	#FF8CD2CD$#��../Fonts/#Montserrat SemiBold)�6,   $��30��6k   $	��103��6w   $��16(�6�   $��
10 0 200 0q�6�   .��6�   
Выйти6�   �� 5      
���   �$��MISSIONS��6+   $#��../Fonts/#Montserrat SemiBold)�6;   $��55(�6f   $��6 7 0 0q�6t   �� 5      ���	#FFFFBE49$��MISSIONS��6/   $#��../Fonts/#Montserrat SemiBold)�6?   $��55(�6j   $��
-285,5,0,0q�6x   �� 5      $	��325��6   $��0 10 0 0q�6%   .+�5      �� -   5         ��Name$ logBox��5      $��Right=�6   $#��../Fonts/#Montserrat SemiBold)�68   $��
Логин��6c   $��16(�6q   6   �� 5      -   5        $ emailBox��5      $��Right=�6   $#��../Fonts/#Montserrat SemiBold)�68   $��
Почта��6c   $��16(�6q   6   5      5      �� 5   
   $
��Auto=�6   $��Bottom=�6:   .��5      � -   5         �Name$ 
UsersPanel��5      #���   6,   	 � Orientation$	 Vertical=�6S   $��10q�6j   5      5      5      