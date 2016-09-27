Partial Public Class MainWindow
    Inherits Window

    Private fixedCircle As Ellipse
    Private movingCircle As Ellipse
    Private penLine As Polyline
    Private gridLines As List(Of Polyline)
    Private p1 As Polyline

    Private showCircles As Boolean
    Private showGrid As Boolean

    Private type As Trochoid.TrochoidType

    Private doneLoading As Boolean
    Public Property pl As Polyline
    ' TODO:
    ' arbitrary number of Polyline objects
    ' ability to add new ones

    Public Sub New()
        InitializeComponent()
        doneLoading = False
        showCircles = True
        showGrid = True
        type = Trochoid.TrochoidType.Hypotrochoid

        createCirclesAndLine()
    End Sub

    Private Sub Window_ContentRendered(sender As Object, e As EventArgs)
        doneLoading = True
        createGridLines()
        setCircleAndLinePositions()
        showHideCirclesAndLine()
        showHideGrid()
    End Sub

    Private Sub createGridLines()
        Dim canvasCenterX As Integer = CType(Convert.ToInt32(renderCanvas.ActualWidth) / 2, Integer)
        Dim canvasCenterY As Integer = CType(Convert.ToInt32(renderCanvas.ActualHeight) / 2, Integer)

        gridLines = New List(Of Polyline)()
        ' horiz line
        gridLines.Add(New Polyline() With {
            .Stroke = New SolidColorBrush(Colors.Gray),
            .StrokeThickness = 1,
            .Points = New PointCollection() From {
                New Point(canvasCenterX, 0),
                New Point(canvasCenterX, renderCanvas.ActualHeight)
            }
        })
        ' vert line
        gridLines.Add(New Polyline() With {
            .Stroke = New SolidColorBrush(Colors.Gray),
            .StrokeThickness = 1,
            .Points = New PointCollection() From {
                New Point(0, canvasCenterY),
                New Point(renderCanvas.ActualWidth, canvasCenterY)
            }
        })

        For Each p As Polyline In gridLines
            renderCanvas.Children.Add(p)
        Next

    End Sub


    Private Sub createCirclesAndLine()
        fixedCircle = New Ellipse() With {
            .Uid = "fixedCircle",
            .Stroke = New SolidColorBrush(Colors.DarkBlue),
            .StrokeThickness = 1
        }
        movingCircle = New Ellipse() With {
            .Uid = "movingCircle",
            .Stroke = New SolidColorBrush(Colors.DarkGreen),
            .StrokeThickness = 1
        }
        penLine = New Polyline() With {
            .Uid = "penLine",
            .Stroke = New SolidColorBrush(Colors.DarkRed),
            .StrokeThickness = 1
        }
        Me.renderCanvas.Children.Add(fixedCircle)
        Me.renderCanvas.Children.Add(movingCircle)
        Me.renderCanvas.Children.Add(penLine)
    End Sub

    Private Sub setCircleAndLinePositions()
        If doneLoading Then
            Dim canvasCenterX As Integer = CType(Convert.ToInt32(renderCanvas.ActualWidth) / 2, Integer)
            Dim canvasCenterY As Integer = CType(Convert.ToInt32(renderCanvas.ActualHeight) / 2, Integer)

            Dim fixedR As Integer = Convert.ToInt32(Me.FixedRadius.Value)
            Dim movingR As Integer = Convert.ToInt32(Me.MovingRadius.Value)
            Dim penD As Integer = Convert.ToInt32(Me.PenDistance.Value)

            ' set top left position of the fixed circle
            Dim fixedCircleTop As Integer = canvasCenterY - fixedR
            Dim fixedCircleLeft As Integer = canvasCenterX - fixedR
            Dim movingCircleTop As Integer
            Dim movingCircleLeft As Integer
            Dim penLineY As Integer = canvasCenterY
            Dim penLineX1 As Integer, penLineX2 As Integer


            If type = Trochoid.TrochoidType.Hypotrochoid Then
                ' set top left position of the moving circle and line
                movingCircleTop = canvasCenterY - movingR
                movingCircleLeft = canvasCenterX + fixedR - 2 * movingR
                penLineX1 = canvasCenterX + fixedR - movingR
                penLineX2 = penLineX1 + penD
            Else
                ' set top left position of the moving circle and line                    
                movingCircleTop = canvasCenterY - movingR
                movingCircleLeft = canvasCenterX + fixedR
                penLineX1 = canvasCenterX + fixedR + movingR
                penLineX2 = penLineX1 + penD
            End If

            ' now update the ellipse and line objects...
            If fixedCircle IsNot Nothing Then
                fixedCircle.Width = 2 * fixedR
                fixedCircle.Height = 2 * fixedR
                Canvas.SetTop(fixedCircle, fixedCircleTop)
                Canvas.SetLeft(fixedCircle, fixedCircleLeft)
            End If
            If movingCircle IsNot Nothing Then
                movingCircle.Width = 2 * movingR
                movingCircle.Height = 2 * movingR
                Canvas.SetTop(movingCircle, movingCircleTop)
                Canvas.SetLeft(movingCircle, movingCircleLeft)
            End If
            If penLine IsNot Nothing Then
                penLine.Points = New PointCollection()
                penLine.Points.Add(New Point(penLineX1, penLineY))
                penLine.Points.Add(New Point(penLineX2, penLineY))
            End If
        End If
    End Sub
    Private Sub showHideGrid()
        If doneLoading Then
            If showGrid Then
                For Each p As Polyline In gridLines
                    p.Visibility = Visibility.Visible
                Next
            Else
                For Each p As Polyline In gridLines
                    p.Visibility = Visibility.Hidden
                Next
            End If
        End If
    End Sub

    Private Sub showHideCirclesAndLine()
        If doneLoading Then
            If showCircles Then
                fixedCircle.Visibility = Visibility.Visible
                movingCircle.Visibility = Visibility.Visible


                penLine.Visibility = Visibility.Visible
            Else
                fixedCircle.Visibility = Visibility.Hidden
                movingCircle.Visibility = Visibility.Hidden
                penLine.Visibility = Visibility.Hidden
            End If
        End If
    End Sub

    Private Sub Draw_Click(sender As Object, e As RoutedEventArgs)
        Dim centerX As Integer = CType(Convert.ToInt32(renderCanvas.ActualWidth) / 2, Integer)
        Dim centerY As Integer = CType(Convert.ToInt32(renderCanvas.ActualHeight) / 2, Integer)


        Dim fixedR As Integer = Convert.ToInt32(Me.FixedRadius.Value)
        Dim movingR As Integer = Convert.ToInt32(Me.MovingRadius.Value)
        Dim penD As Integer = Convert.ToInt32(Me.PenDistance.Value)

        ' get color...
        Dim c As Color = Colors.Black
        If Me.PenColor.SelectedColor.HasValue Then
            c = Me.PenColor.SelectedColor.Value
        End If

        ' now get the points
        Dim p As List(Of Point)
        p = Trochoid.GetPoints(fixedR, movingR, penD, centerX, centerY, type)

        ' create the polyline
        Me.pl = New Polyline() With {
            .Uid = "PolyLine",
            .StrokeThickness = Me.PenThickness.Value,
            .Stroke = New SolidColorBrush(c),
            .Points = New PointCollection(p)
        }
        Me.renderCanvas.Children.Add(pl)

    End Sub


    Private Sub Clear_Click(sender As Object, e As RoutedEventArgs)
        removeItemsFromCanvasByUid("PolyLine")

        showHideCirclesAndLine()
        showHideGrid()
    End Sub

    Private Sub removeItemsFromCanvasByUid(uid As String)
        Dim removeObjects As New List(Of UIElement)()
        For Each u As UIElement In renderCanvas.Children

            If u.Uid.StartsWith(uid) Then
                removeObjects.Add(u)
            End If
        Next
        For Each u As UIElement In removeObjects
            renderCanvas.Children.Remove(u)
        Next
    End Sub

    Private Sub FixedRadius_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        setCircleAndLinePositions()
    End Sub

    Private Sub MovingRadius_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        setCircleAndLinePositions()
    End Sub

    Private Sub PenDistance_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        setCircleAndLinePositions()
    End Sub

    Private Sub Hypotrochoid_Checked(sender As Object, e As RoutedEventArgs)
        type = Trochoid.TrochoidType.Hypotrochoid
        setCircleAndLinePositions()
        showHideCirclesAndLine()


    End Sub

    Private Sub Epitrochoid_Checked(sender As Object, e As RoutedEventArgs)
        type = Trochoid.TrochoidType.Epitrochoid
        setCircleAndLinePositions()
        showHideCirclesAndLine()

    End Sub

    Private Sub hideCircles_Checked(sender As Object, e As RoutedEventArgs)
        showCircles = False
        showHideCirclesAndLine()
    End Sub

    Private Sub hideCircles_Unchecked(sender As Object, e As RoutedEventArgs)
        showCircles = True
        showHideCirclesAndLine()

    End Sub

    Private Sub hideGrid_Checked(sender As Object, e As RoutedEventArgs)
        showGrid = False
        showHideGrid()
    End Sub

    Private Sub hideGrid_Unchecked(sender As Object, e As RoutedEventArgs)
        showGrid = True
        showHideGrid()
    End Sub
End Class
