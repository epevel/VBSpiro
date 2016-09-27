
Public Class Trochoid
    ''' <summary>
    ''' The type of trochoid; 
    ''' Hypotrochoid: circle moves inside a fixed circle
    ''' Epitrochoid: circle moves outside a fixed circle
    ''' </summary>
    Public Enum TrochoidType
        Hypotrochoid
        Epitrochoid
    End Enum

    ''' <summary>
    ''' Parametric equations for X of a Trochoid.
    ''' </summary>
    ''' <param name="t">Variable t.</param>
    ''' <param name="fR">Fixed radius.</param>
    ''' <param name="mR">Moving Radius.</param>
    ''' <param name="d">Pen distance from center of moving circle.</param>
    ''' <param name="type">One of Hypotrochoid or Epitrochoid.</param>
    ''' <returns>The X coordinate with relation to t.</returns>
    ''' <remarks>
    ''' https://en.wikipedia.org/wiki/Hypotrochoid
    ''' https://en.wikipedia.org/wiki/Epitrochoid
    ''' </remarks>
    Private Shared Function getX(t As Double, fR As Double, mR As Double, d As Double, type As TrochoidType) As Double
        If type = TrochoidType.Hypotrochoid Then
            Return (fR - mR) * Math.Cos(t) + d * Math.Cos((fR - mR) / mR * t)
        Else
            Return (fR + mR) * Math.Cos(t) - d * Math.Cos((fR + mR) / mR * t)
        End If
    End Function

    ''' <summary>
    ''' Parametric equations for Y of a Trochoid.
    ''' </summary>
    ''' <param name="t">Variable t.</param>
    ''' <param name="fR">Fixed radius.</param>
    ''' <param name="mR">Moving Radius.</param>
    ''' <param name="d">Pen distance from center of moving circle.</param>
    ''' <param name="type">One of Hypotrochoid or Epitrochoid.</param>
    ''' <returns>The Y coordinate with relation to t.</returns>
    ''' <remarks>
    ''' https://en.wikipedia.org/wiki/Hypotrochoid
    ''' https://en.wikipedia.org/wiki/Epitrochoid
    ''' </remarks>
    Private Shared Function getY(t As Double, fR As Double, mR As Double, d As Double, type As TrochoidType) As Double
        If type = TrochoidType.Hypotrochoid Then
            Return (fR - mR) * Math.Sin(t) - d * Math.Sin((fR - mR) / mR * t)
        Else
            Return (fR + mR) * Math.Sin(t) - d * Math.Sin((fR + mR) / mR * t)
        End If
    End Function

    ''' <summary>
    ''' Recursive GCD function.
    ''' </summary>
    ''' <param name="x">1st int</param>
    ''' <param name="y">2nd int</param>
    ''' <returns>The greatest common divisor for the two integers x and y.</returns>
    Private Shared Function greatestCommonDivisor(x As Integer, y As Integer) As Integer
        If x Mod y = 0 Then
            Return y
        Else
            Return greatestCommonDivisor(y, x Mod y)
        End If

    End Function

    Public Shared Function GetPoints(fixedRadius As Integer, movingRadius As Integer, penLength As Integer, centerX As Integer, centerY As Integer, type As TrochoidType) As List(Of Point)
        Return GetPoints(fixedRadius, movingRadius, penLength, 360, centerX, centerY, type)
    End Function

    ''' <summary>
    ''' Given a fixed radius, a moving radius, a pen length and a type find the points that define the coordinates of a trochoid with its center at the coordinates provided.
    ''' </summary>
    ''' <param name="fixedRadius">The fixed radius.</param>
    ''' <param name="movingRadius">The moving radius.</param>
    ''' <param name="penLength">The distance from the center of the moving circle the pen sits at.</param>
    ''' <param name="pointsPerRevolution">How many points to define per revolution; higher == smoother curve.</param>
    ''' <param name="centerX">The x coordinate of the point the trochoid should be centered on.</param>
    ''' <param name="centerY">The y coordinate of the point the trochoid should be centered on.</param>
    ''' <param name="type">One of Hypotrochoid or Epitrochoid.</param>
    ''' <returns>A list of points that represent the coordinates of the trochoid.</returns>
    Public Shared Function GetPoints(fixedRadius As Integer, movingRadius As Integer, penLength As Integer,
                                     pointsPerRevolution As Integer, centerX As Integer, centerY As Integer,
                                     type As TrochoidType) As List(Of Point)
        Dim retVal As New List(Of Point)()
        If fixedRadius <> 0 AndAlso movingRadius <> 0 Then
            Dim gcd As Integer = greatestCommonDivisor(fixedRadius, movingRadius)

            ' starting variable
            Dim t As Double = 0
            ' increment; more PointsPerRevolution == smaller increment == smoother curve
            Dim increment As Double = Math.PI / pointsPerRevolution

            ' the limit of the variable T;  the circumference of the moving circle / GCD of both circles
            Dim maxT As Double = (2 * Math.PI) * movingRadius / gcd

            ' get the starting point
            Dim x As Double = centerX + Trochoid.getX(t, fixedRadius, movingRadius, penLength, type)
            Dim y As Double = centerY + Trochoid.getY(t, fixedRadius, movingRadius, penLength, type)
            ' add 1st point
            retVal.Add(New Point(x, y))

            ' get the values of x and y as t goes from 0 to maxT
            While t <= maxT
                t += increment
                x = centerX + Trochoid.getX(t, fixedRadius, movingRadius, penLength, type)
                y = centerY + Trochoid.getY(t, fixedRadius, movingRadius, penLength, type)
                retVal.Add(New Point(x, y))
            End While
        End If
        Return retVal

    End Function

End Class

