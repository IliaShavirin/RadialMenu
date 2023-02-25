using System;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg
{
    /// <summary>
    ///     This is an implementation of the 3-by-3 affine matrix that represents
    ///     a geometric transform.
    /// </summary>
    public class SvgTransformF : ICloneable
    {
        #region Private Fields

        private float m11;
        private float m12;
        private float m21;
        private float m22;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        ///     Initializes a new instance of the <see cref="SvgTransformF" /> class.
        /// </overloads>
        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgTransformF" /> class
        ///     as the identity transform or matrix.
        /// </summary>
        public SvgTransformF()
        {
            m11 = 1.0f;
            m12 = 0.0f;
            m21 = 0.0f;
            m22 = 1.0f;
            OffsetX = 0.0f;
            OffsetY = 0.0f;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgTransformF" /> class
        ///     to the geometric transform defined by the specified rectangle and
        ///     array of points.
        /// </summary>
        /// <param name="rect">
        ///     A <see cref="SvgRectF" /> structure that represents the rectangle
        ///     to be transformed.
        /// </param>
        /// <param name="plgpts">
        ///     An array of three <see cref="SvgPointF" /> structures that represents the
        ///     points of a parallelogram to which the upper-left, upper-right, and
        ///     lower-left corners of the rectangle is to be transformed. The
        ///     lower-right corner of the parallelogram is implied by the first three
        ///     corners.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="plgpts" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the length of the <paramref name="plgpts" /> array is not equal
        ///     to 3.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If the width or height of the <paramref name="rect" /> is zero.
        /// </exception>
        public SvgTransformF(SvgRectF rect, SvgPointF[] plgpts)
        {
            if (plgpts == null) throw new ArgumentNullException("plgpts");
            if (plgpts.Length != 3) throw new ArgumentException("plgpts");

            if (rect.Width == 0 || rect.Height == 0) throw new ArgumentOutOfRangeException("rect");

            MapRectToRect(rect, plgpts);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgTransformF" /> class
        ///     with the specified elements.
        /// </summary>
        /// <param name="elements">
        ///     An array of six items defining the transform.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="elements" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the length of the <paramref name="elements" /> array is not equal
        ///     to 6.
        /// </exception>
        public SvgTransformF(float[] elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length != 6) throw new ArgumentException("elements");

            m11 = elements[0];
            m12 = elements[1];
            m21 = elements[2];
            m22 = elements[3];
            OffsetX = elements[4];
            OffsetY = elements[5];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgTransformF" /> class
        ///     with the specified elements.
        /// </summary>
        /// <param name="m11">
        ///     The value in the first row and first column of the new <see cref="SvgTransformF" />.
        /// </param>
        /// <param name="m12">
        ///     The value in the first row and second column of the new <see cref="SvgTransformF" />.
        /// </param>
        /// <param name="m21">
        ///     The value in the second row and first column of the new <see cref="SvgTransformF" />.
        /// </param>
        /// <param name="m22">
        ///     The value in the second row and second column of the new <see cref="SvgTransformF" />.
        /// </param>
        /// <param name="dx">
        ///     The value in the third row and first column of the new <see cref="SvgTransformF" />.
        /// </param>
        /// <param name="dy">
        ///     The value in the third row and second column of the new <see cref="SvgTransformF" />.
        /// </param>
        public SvgTransformF(float m11, float m12, float m21, float m22,
            float dx, float dy)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
            OffsetX = dx;
            OffsetY = dy;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgTransformF" /> class
        ///     with parameters copied from the specified parameter, a copy
        ///     constructor.
        /// </summary>
        /// <param name="source">
        ///     The <see cref="SvgTransformF" /> instance from which the parameters
        ///     are to be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="source" /> is <see langword="null" />.
        /// </exception>
        public SvgTransformF(SvgTransformF source)
        {
            if (source == null) throw new ArgumentNullException("source");

            m11 = source.m11;
            m12 = source.m12;
            m21 = source.m21;
            m22 = source.m22;
            OffsetX = source.OffsetX;
            OffsetY = source.OffsetY;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets an array of floating-point values that represents the elements
        ///     of this <see cref="SvgTransformF" />.
        /// </summary>
        /// <value>
        ///     An array of floating-point values that represents the elements
        ///     of this <see cref="SvgTransformF" />.
        /// </value>
        public float[] Elements
        {
            get { return new[] { m11, m12, m21, m22, OffsetX, OffsetY }; }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="SvgTransformF" /> is the
        ///     identity matrix.
        /// </summary>
        /// <value>
        ///     This property is <see langword="true" /> if this
        ///     <see cref="SvgTransformF" /> is identity; otherwise, <see langword="false" />.
        /// </value>
        public bool IsIdentity =>
            m11 == 1.0f && m12 == 0.0f &&
            m21 == 0.0f && m22 == 1.0f &&
            OffsetX == 0.0f && OffsetY == 0.0f;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="SvgTransformF" /> is
        ///     invertible.
        /// </summary>
        /// <value>
        ///     This property is <see langword="true" /> if this
        ///     <see cref="SvgTransformF" /> is invertible; otherwise, <see langword="false" />.
        /// </value>
        public bool IsInvertible => m11 * m22 - m21 * m11 != 0.0f;

        /// <summary>
        ///     Gets the <c>x</c> translation value (the <c>dx</c> value, or the
        ///     element in the third row and first column) of this <see cref="SvgTransformF" />.
        /// </summary>
        /// <value>
        ///     The <c>x</c> translation value of this <see cref="SvgTransformF" />.
        /// </value>
        public float OffsetX { get; private set; }

        /// <summary>
        ///     Gets the <c>y</c> translation value (the <c>dy</c> value, or the
        ///     element in the third row and second column) of this <see cref="SvgTransformF" />.
        /// </summary>
        /// <value>
        ///     The <c>y</c> translation value of this <see cref="SvgTransformF" />.
        /// </value>
        public float OffsetY { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determine whether the specified object is a <see cref="SvgTransformF" />
        ///     and is identical to this <see cref="SvgTransformF" />.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>
        ///     This method returns <see langword="true" /> if obj is the specified
        ///     <see cref="SvgTransformF" /> identical to this
        ///     <see cref="SvgTransformF" />; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as SvgTransformF;
            if (other != null)
                return other.m11 == m11 && other.m12 == m12 &&
                       other.m21 == m21 && other.m22 == m22 &&
                       other.OffsetX == OffsetX && other.OffsetY == OffsetY;

            return false;
        }

        /// <summary>
        ///     Returns a hash code.
        /// </summary>
        /// <returns>The hash code for this <see cref="SvgTransformF" />.</returns>
        public override int GetHashCode()
        {
            return (int)(m11 + m12 + m21 + m22 + OffsetX + OffsetY);
        }

        /// <summary>
        ///     Inverts this <see cref="SvgTransformF" />, if it is invertible.
        /// </summary>
        public void Invert()
        {
            var determinant = m11 * m22 - m21 * m11;
            if (determinant != 0.0f)
            {
                var nm11 = m22 / determinant;
                var nm12 = -(m12 / determinant);
                var nm21 = -(m21 / determinant);
                var nm22 = m11 / determinant;
                var ndx = (m12 * OffsetY - m22 * OffsetX) / determinant;
                var ndy = (m21 * OffsetX - m11 * OffsetY) / determinant;

                m11 = nm11;
                m12 = nm12;
                m21 = nm21;
                m22 = nm22;
                OffsetX = ndx;
                OffsetY = ndy;
            }
        }

        /// <overloads>
        ///     Multiplies this <see cref="SvgTransformF" /> by the specified
        ///     <see cref="SvgTransformF" /> by appending or prepending the specified
        ///     <see cref="SvgTransformF" />.
        /// </overloads>
        /// <summary>
        ///     Multiplies this <see cref="SvgTransformF" /> by the specified
        ///     <see cref="SvgTransformF" /> by prepending the specified
        ///     <see cref="SvgTransformF" />.
        /// </summary>
        /// <param name="matrix">
        ///     The <see cref="SvgTransformF" /> by which this <see cref="SvgTransformF" />
        ///     is to be multiplied.
        /// </param>
        public void Multiply(SvgTransformF matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            Multiply(matrix, this);
        }

        /// <summary>
        ///     Multiplies this <see cref="SvgTransformF" /> by the matrix specified in
        ///     the matrix parameter, and in the order specified in the order parameter.
        /// </summary>
        /// <param name="matrix">
        ///     The <see cref="SvgTransformF" /> by which this <see cref="SvgTransformF" />
        ///     is to be multiplied.
        /// </param>
        /// <param name="order">
        ///     The <see cref="TransformOrder" /> that represents the order of the
        ///     multiplication.
        /// </param>
        public void Multiply(SvgTransformF matrix, SvgTransformOrder order)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            if (order == SvgTransformOrder.Prepend)
                Multiply(matrix, this);
            else
                Multiply(this, matrix);
        }

        /// <summary>
        /// Multiplies this <see cref="SvgTransformF"/> by the specified 
        /// <see cref="SvgTransformF"/> by prepending the specified 
        /// <see cref="SvgTransformF"/>.
        /// </summary>
        /// <param name="matrix">
        /// The <see cref="SvgTransformF"/> by which this <see cref="SvgTransformF"/> 
        /// is to be multiplied.
        /// </param>
        //public void Multiply(SvgTransformF matrix)
        //{
        //    if (matrix == null)
        //    {
        //        throw new ArgumentNullException("matrix");
        //    }
        //    Multiply(matrix, this);
        //}

        /// <summary>
        /// Multiplies this <see cref="SvgTransformF"/> by the matrix specified in 
        /// the matrix parameter, and in the order specified in the order parameter.
        /// </summary>
        /// <param name="matrix">
        /// The <see cref="SvgTransformF"/> by which this <see cref="SvgTransformF"/> 
        /// is to be multiplied.
        /// </param>
        /// <param name="order">
        /// The <see cref="TransformOrder"/> that represents the order of the 
        /// multiplication.
        /// </param>
        //public void Multiply(SvgTransformF matrix, TransformOrder order)
        //{
        //    if (matrix == null)
        //    {
        //        throw new ArgumentNullException("matrix");
        //    }
        //    if (order == TransformOrder.Prepend)
        //    {
        //        Multiply(matrix, this);
        //    }
        //    else
        //    {
        //        Multiply(this, matrix);
        //    }
        //}

        /// <summary>
        ///     Resets this <see cref="SvgTransformF" /> to have the elements of the
        ///     identity matrix.
        /// </summary>
        public void Reset()
        {
            m11 = 1.0f;
            m12 = 0.0f;
            m21 = 0.0f;
            m22 = 0.1f;
            OffsetX = 0.0f;
            OffsetY = 0.0f;
        }

        /// <overloads>
        ///     Applies a clockwise rotation of the specified angle about the
        ///     origin to this <see cref="SvgTransformF" />.
        /// </overloads>
        /// <summary>
        ///     Applies a clockwise rotation of the specified angle about the
        ///     origin to this <see cref="SvgTransformF" />.
        /// </summary>
        /// <param name="angle">
        ///     The angle (extent) of the rotation, in degrees.
        /// </param>
        public void Rotate(float angle)
        {
            var radians = angle * (Math.PI / 180.0);
            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);

            var nm11 = cos * m11 + sin * m21;
            var nm12 = cos * m12 + sin * m22;
            var nm21 = cos * m21 - sin * m11;
            var nm22 = cos * m22 - sin * m12;

            m11 = nm11;
            m12 = nm12;
            m21 = nm21;
            m22 = nm22;
        }

        /// <summary>
        ///     Applies a clockwise rotation of the specified angle about the
        ///     origin to this <see cref="SvgTransformF" />, and in the order specified
        ///     in the order parameter.
        /// </summary>
        /// <param name="angle">
        ///     The angle (extent) of the rotation, in degrees.
        /// </param>
        /// <param name="order">
        ///     A <see cref="TransformOrder" /> that specifies the order (append or
        ///     prepend) in which the rotation is applied to this
        ///     <see cref="SvgTransformF" />.
        /// </param>
        public void Rotate(float angle, SvgTransformOrder order)
        {
            var radians = angle * (Math.PI / 180.0);
            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);

            if (order == SvgTransformOrder.Prepend)
            {
                var nm11 = cos * m11 + sin * m21;
                var nm12 = cos * m12 + sin * m22;
                var nm21 = cos * m21 - sin * m11;
                var nm22 = cos * m22 - sin * m12;

                m11 = nm11;
                m12 = nm12;
                m21 = nm21;
                m22 = nm22;
            }
            else
            {
                var nm11 = m11 * cos - m12 * sin;
                var nm12 = m11 * sin + m12 * cos;
                var nm21 = m21 * cos - m22 * sin;
                var nm22 = m21 * sin + m22 * cos;
                var ndx = OffsetX * cos - OffsetY * sin;
                var ndy = OffsetX * sin + OffsetY * cos;

                m11 = nm11;
                m12 = nm12;
                m21 = nm21;
                m22 = nm22;
                OffsetX = ndx;
                OffsetY = ndy;
            }
        }

        /// <overloads>
        ///     Applies a clockwise rotation about the specified point to this
        ///     <see cref="SvgTransformF" /> by appending or prepending the rotation.
        /// </overloads>
        /// <summary>
        ///     Applies a clockwise rotation about the specified point to this
        ///     <see cref="SvgTransformF" /> by prepending the rotation.
        /// </summary>
        /// <param name="angle">
        ///     The angle (extent) of the rotation, in degrees.
        /// </param>
        /// <param name="point">
        ///     A <see cref="SvgPointF" /> that represents the center of the rotation.
        /// </param>
        public void RotateAt(float angle, SvgPointF point)
        {
            Translate(point.X, point.Y);
            Rotate(angle);
            Translate(-point.X, -point.Y);
        }

        /// <summary>
        ///     Applies a clockwise rotation about the specified point to this
        ///     <see cref="SvgTransformF" /> in the specified order.
        /// </summary>
        /// <param name="angle">
        ///     The angle (extent) of the rotation, in degrees.
        /// </param>
        /// <param name="point">
        ///     A <see cref="SvgPointF" /> that represents the center of the rotation.
        /// </param>
        /// <param name="order">
        ///     A <see cref="TransformOrder" /> that specifies the order (append or
        ///     prepend) in which the rotation is applied.
        /// </param>
        public void RotateAt(float angle, SvgPointF point, SvgTransformOrder order)
        {
            if (order == SvgTransformOrder.Prepend)
            {
                Translate(point.X, point.Y);
                Rotate(angle);
                Translate(-point.X, -point.Y);
            }
            else
            {
                Translate(-point.X, -point.Y);
                Rotate(angle, SvgTransformOrder.Append);
                Translate(point.X, point.Y);
            }
        }

        /// <overloads>
        ///     Applies the specified scale vector to this <see cref="SvgTransformF" />
        ///     by appending or prepending the scale vector.
        /// </overloads>
        /// <summary>
        ///     Applies the specified scale vector to this <see cref="SvgTransformF" />
        ///     by prepending the scale vector.
        /// </summary>
        /// <param name="scaleX">
        ///     The value by which to scale this <see cref="SvgTransformF" /> in the
        ///     x-axis direction.
        /// </param>
        /// <param name="scaleY">
        ///     The value by which to scale this <see cref="SvgTransformF" /> in the
        ///     y-axis direction.
        /// </param>
        public void Scale(float scaleX, float scaleY)
        {
            m11 *= scaleX;
            m12 *= scaleX;
            m21 *= scaleY;
            m22 *= scaleY;
        }

        /// <summary>
        ///     Applies the specified scale vector to this <see cref="SvgTransformF" />
        ///     using the specified order.
        /// </summary>
        /// <param name="scaleX">
        ///     The value by which to scale this <see cref="SvgTransformF" /> in the
        ///     x-axis direction.
        /// </param>
        /// <param name="scaleY">
        ///     The value by which to scale this <see cref="SvgTransformF" /> in the
        ///     y-axis direction.
        /// </param>
        /// <param name="order">
        ///     A <see cref="TransformOrder" /> that specifies the order (append or
        ///     prepend) in which the scale vector is applied to this
        ///     <see cref="SvgTransformF" />.
        /// </param>
        public void Scale(float scaleX, float scaleY, SvgTransformOrder order)
        {
            if (order == SvgTransformOrder.Prepend)
            {
                m11 *= scaleX;
                m12 *= scaleX;
                m21 *= scaleY;
                m22 *= scaleY;
            }
            else
            {
                m11 *= scaleX;
                m12 *= scaleY;
                m21 *= scaleX;
                m22 *= scaleY;
                OffsetX *= scaleX;
                OffsetY *= scaleY;
            }
        }

        /// <overloads>
        ///     Applies the specified shear vector to this <see cref="SvgTransformF" /> by
        ///     appending or prepending the shear vector.
        /// </overloads>
        /// <summary>
        ///     Applies the specified shear vector to this <see cref="SvgTransformF" /> by
        ///     prepending the shear vector.
        /// </summary>
        /// <param name="shearX">
        ///     The horizontal shear factor.
        /// </param>
        /// <param name="shearY">
        ///     The vertical shear factor.
        /// </param>
        public void Shear(float shearX, float shearY)
        {
            var nm11 = m11 + m21 * shearY;
            var nm12 = m12 + m22 * shearY;
            var nm21 = m11 * shearX + m21;
            var nm22 = m12 * shearX + m22;

            m11 = nm11;
            m12 = nm12;
            m21 = nm21;
            m22 = nm22;
        }

        /// <summary>
        ///     Applies the specified shear vector to this <see cref="SvgTransformF" /> in
        ///     the specified order.
        /// </summary>
        /// <param name="shearX">
        ///     The horizontal shear factor.
        /// </param>
        /// <param name="shearY">
        ///     The vertical shear factor.
        /// </param>
        /// <param name="order">
        ///     A <see cref="TransformOrder" /> that specifies the order (append or
        ///     prepend) in which the shear is applied.
        /// </param>
        public void Shear(float shearX, float shearY, SvgTransformOrder order)
        {
            if (order == SvgTransformOrder.Prepend)
            {
                var nm11 = m11 + m21 * shearY;
                var nm12 = m12 + m22 * shearY;
                var nm21 = m11 * shearX + m21;
                var nm22 = m12 * shearX + m22;

                m11 = nm11;
                m12 = nm12;
                m21 = nm21;
                m22 = nm22;
            }
            else
            {
                var nm11 = m11 + m12 * shearX;
                var nm12 = m11 * shearY + m12;
                var nm21 = m21 + m22 * shearX;
                var nm22 = m21 * shearY + m22;
                var ndx = OffsetX + OffsetY * shearX;
                var ndy = OffsetX * shearY + OffsetY;

                m11 = nm11;
                m12 = nm12;
                m21 = nm21;
                m22 = nm22;
                OffsetX = ndx;
                OffsetY = ndy;
            }
        }

        /// <overloads>
        ///     Applies the specified translation vector to this <see cref="SvgTransformF" />
        ///     by appending or prepending the translation vector.
        /// </overloads>
        /// <summary>
        ///     Applies the specified translation vector to this <see cref="SvgTransformF" />
        ///     by prepending the translation vector.
        /// </summary>
        /// <param name="offsetX">
        ///     The <c>x</c> value by which to translate this <see cref="SvgTransformF" />.
        /// </param>
        /// <param name="offsetY">
        ///     The <c>y</c> value by which to translate this <see cref="SvgTransformF" />.
        /// </param>
        public void Translate(float offsetX, float offsetY)
        {
            OffsetX += offsetX * m11 + offsetY * m21;
            OffsetY += offsetX * m12 + offsetY * m22;
        }

        /// <summary>
        ///     Applies the specified translation vector to this <see cref="SvgTransformF" />
        ///     in the specified order.
        /// </summary>
        /// <param name="offsetX">
        ///     The <c>x</c> value by which to translate this <see cref="SvgTransformF" />.
        /// </param>
        /// <param name="offsetY">
        ///     The <c>y</c> value by which to translate this <see cref="SvgTransformF" />.
        /// </param>
        /// <param name="order">
        ///     A <see cref="TransformOrder" /> that specifies the order (append or
        ///     prepend) in which the translation is applied to this <see cref="SvgTransformF" />.
        /// </param>
        public void Translate(float offsetX, float offsetY, SvgTransformOrder order)
        {
            if (order == SvgTransformOrder.Prepend)
            {
                OffsetX += offsetX * m11 + offsetY * m21;
                OffsetY += offsetX * m12 + offsetY * m22;
            }
            else
            {
                OffsetX += offsetX;
                OffsetY += offsetY;
            }
        }

        /// <summary>
        ///     Applies the geometric transform represented by this
        ///     <see cref="SvgTransformF" /> to a specified point.
        /// </summary>
        /// <param name="x">The input <c>x</c> value of the point.</param>
        /// <param name="y">The input <c>y</c> value of the point.</param>
        /// <param name="ox">The transformed <c>x</c> value of the point.</param>
        /// <param name="oy">The transformed <c>y</c> value of the point.</param>
        public void Transform(float x, float y, out float ox, out float oy)
        {
            ox = x * m11 + y * m21 + OffsetX;
            oy = x * m12 + y * m22 + OffsetY;
        }

        /// <summary>
        ///     Applies the reverse geometric transform represented by this
        ///     <see cref="SvgTransformF" /> to a specified point.
        /// </summary>
        /// <param name="x">The input <c>x</c> value of the point.</param>
        /// <param name="y">The input <c>y</c> value of the point.</param>
        /// <param name="ox">The transformed <c>x</c> value of the point.</param>
        /// <param name="oy">The transformed <c>y</c> value of the point.</param>
        public void ReverseTransform(float x, float y, out float ox, out float oy)
        {
            var determinant = m11 * m22 - m21 * m11;
            if (determinant != 0.0f)
            {
                var nm11 = m22 / determinant;
                var nm12 = -(m12 / determinant);
                var nm21 = -(m21 / determinant);
                var nm22 = m11 / determinant;

                ox = x * nm11 + y * nm21;
                oy = x * nm12 + y * nm22;
            }
            else
            {
                ox = x;
                oy = y;
            }
        }

        /// <summary>
        ///     Applies the geometric transform represented by this
        ///     <see cref="SvgTransformF" /> to a specified array of points.
        /// </summary>
        /// <param name="pts">
        ///     An array of <see cref="SvgPointF" /> structures that represents the points
        ///     to transform.
        /// </param>
        public void TransformPoints(SvgPointF[] pts)
        {
            if (pts == null) throw new ArgumentNullException("pts");

            var nLength = pts.Length;

            for (var i = nLength - 1; i >= 0; --i)
            {
                var x = pts[i].X;
                var y = pts[i].Y;
                pts[i].ValueX = x * m11 + y * m21 + OffsetX;
                pts[i].ValueY = x * m12 + y * m22 + OffsetY;
            }
        }

        /// <summary>
        ///     Multiplies each vector in an array by the matrix. The translation
        ///     elements of this matrix (third row) are ignored.
        /// </summary>
        /// <param name="pts">
        ///     An array of <see cref="SvgPointF" /> structures that represents the points
        ///     to transform.
        /// </param>
        public void TransformVectors(SvgPointF[] pts)
        {
            if (pts == null) throw new ArgumentNullException("pts");

            var nLength = pts.Length;

            for (var i = nLength - 1; i >= 0; --i)
            {
                var x = pts[i].X;
                var y = pts[i].Y;
                pts[i].ValueX = x * m11 + y * m21;
                pts[i].ValueY = x * m12 + y * m22;
            }
        }

        #endregion

        #region Private Methods

        private void Multiply(SvgTransformF a, SvgTransformF b)
        {
            var nm11 = a.m11 * b.m11 + a.m12 * b.m21;
            var nm12 = a.m11 * b.m12 + a.m12 * b.m22;
            var nm21 = a.m21 * b.m11 + a.m22 * b.m21;
            var nm22 = a.m21 * b.m12 + a.m22 * b.m22;
            var ndx = a.OffsetX * b.m11 + a.OffsetY * b.m21 + b.OffsetX;
            var ndy = a.OffsetX * b.m12 + a.OffsetY * b.m22 + b.OffsetY;

            m11 = nm11;
            m12 = nm12;
            m21 = nm21;
            m22 = nm22;
            OffsetX = ndx;
            OffsetY = ndy;
        }

        private void MapRectToRect(SvgRectF rect, SvgPointF[] plgpts)
        {
            var pt1 = new SvgPointF(plgpts[1].X - plgpts[0].X,
                plgpts[1].Y - plgpts[0].Y);
            var pt2 = new SvgPointF(plgpts[2].X - plgpts[0].X,
                plgpts[2].Y - plgpts[0].Y);

            m11 = pt1.X / rect.Width;
            m12 = pt1.Y / rect.Width;
            m21 = pt2.X / rect.Height;
            m22 = pt2.Y / rect.Height;
            OffsetX = plgpts[0].X - rect.X / rect.Width * pt1.X - rect.Y / rect.Height * pt2.X;
            OffsetY = plgpts[0].Y - rect.X / rect.Width * pt1.Y - rect.Y / rect.Height * pt2.Y;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        ///     This creates a new <see cref="SvgTransformF" /> that is a deep
        ///     copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public SvgTransformF Clone()
        {
            return new SvgTransformF(m11, m12,
                m21, m22,
                OffsetX, OffsetY);
        }

        /// <summary>
        /// This creates a new <see cref="SvgTransformF"/> that is a deep 
        /// copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        //ITransform ITransform.Clone()
        //{
        //    return this.Clone();
        //}

        /// <summary>
        ///     This creates a new <see cref="SvgTransformF" /> that is a deep
        ///     copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}